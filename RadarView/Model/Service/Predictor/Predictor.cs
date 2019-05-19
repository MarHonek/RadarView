using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Render;
using RadarView.Model.Service.Utils;
using RadarView.Properties;
using RadarView.Utils;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.Model.Service.Predictor
{
	/// <summary>
	/// Třída reprezentuje prediktor, který dopočítává informace o letadle a předpovídá polohu letadla.
	/// </summary>
	public class Predictor
	{
		/// <summary>
		/// Časový úsek pro výpočet zda letadlo stoupá/klesá.
		/// Rychlost je vypočtena z hodnot nadmořské výšku v daném časovém úseku.
		/// </summary>
		private readonly int VerticalSpeedTimeDiff = Settings.Default.PredictorVerticalSpeedTimeDiff;

		/// <summary>
		/// Práh odchylky pro určení zda letadlo stoupá/klesá/drží výšku. (m/s).
		/// Pokud je rychlost stoupání/klesání pod touto hodnotou, predictor určí, že letadlo drží výšku.
		/// </summary>
		private readonly float VerticalSpeedThreshold = Settings.Default.PredictorVerticalSpeedThreshold;

		/// <summary>
		/// Čas určující délku ukazatele směru a rychlosti.
		/// Hodnota v sekundách určuje kde se bude letadlo nacházet.
		/// </summary>
		private readonly int SpeedVectorTimeLength = Settings.Default.AircraftSpeedVectorTimeLengthSeconds;

		/// <summary>
		/// Maximální stáří polohové informace, předtím než bude zahozena (sekundy).
		/// </summary>
		private readonly int RealFixTimeout = Settings.Default.AircraftRealFixTimeout;

		/// <summary>
		/// Počet sekund, který určuje jaké fixy budou použity pro extrapolaci.
		/// Hledáme fixy, které mají čas (nejmladší reálný fix - MinTimeIntervalForPrediction).
		/// </summary>
		private readonly int MinTimeIntervalForPrediction = Settings.Default.PredictorMinTimeIntervaForPredictionSeconds;

		/// <summary>
		/// Maximální délka času, pro který ještě predikujeme polohu (sekundy).
		/// </summary>
		private readonly int MaxPredictedSeconds = Settings.Default.PredictorMaxPredictedSeconds;

		/// <summary>
		/// Prahová hodnota rozdílu výšky letadla a letiště pro detekci letadla na zemi (metry).
		/// </summary>
		private readonly int AltitudeThresholdForOnGroundDetection = Settings.Default.PredictorAltitudeThresholdForOnGroundDetection;

		/// <summary>
		/// Maximální rychlost letadla pro detekci letadla na zemi (m/s).
		/// </summary>
		private readonly int MaxSpeedForOnGroundDetection = Settings.Default.PredictorMaxGroundSpeedForOnGroundDetection;

		/// <summary>
		/// Minimální počet reálných fixů.
		/// </summary>
		private readonly int MinFixCountForLinearRegresion = Settings.Default.PredictorMinFixCountForLinearRegresion;

		/// <summary>
		/// Maximální poloměr kružnice u které ještě predikujeme polohu.
		/// </summary>
		private readonly float MaxCircleRadiusForFitting = Settings.Default.PredictorMaxRadiusForCirleFitting;

		/// <summary>
		/// Box určuje sledovanou oblast.
		/// </summary>
		private BoundingBox monitoredArea;

		/// <summary>
		/// Vytvoří novou instanci třídy Predictor.
		/// </summary>
		/// <param name="monitoredArea">Box určující sledovanou oblast.</param>
		public Predictor(BoundingBox monitoredArea)
		{
			this.monitoredArea = monitoredArea;
		}

		/// <summary>
		/// Určí polohu a informace o letadle na základě surových dat.
		/// </summary>
		/// <param name="currentFixDateTime">čas aktuálního fixu.</param>
		/// <param name="trailDotsDateTime">časy historických fixů.</param>
		/// <param name="rawData">Surová data z datového zdroje.</param>
		/// <param name="airportArea">Oblast kolem letiště. Tvoří ji nadmořská výška a hranice oblasti.</param>
		/// <returns>Predikované informace o letadle.</returns>
		public Tuple<PredictorResultEnum, Aircraft> Predict(DateTime currentFixDateTime, DateTime[] trailDotsDateTime, AircraftRawData rawData, Tuple<int, BoundingBox> airportArea)
		{

			if ((currentFixDateTime - rawData.YoungestRealFixDateTime).TotalSeconds > this.RealFixTimeout) {
				//Doba od poslední informace z datového zdroje překročila dobu. Vrátí příznak Remove = letadlo bude odstraněno.
				return new Tuple<PredictorResultEnum, Aircraft>(PredictorResultEnum.Remove, null);
			}

			//Aktuální stavový vektor.
			var currentStateVector = rawData.GetStateVectorOrPrevious(currentFixDateTime);
			if (currentStateVector == null) {
				//Čas oproti serveru je posunuty dozadu. Nemáme informace pro aktuální čas => letadlo zůstane prozatím nezobrazováno.
				return new Tuple<PredictorResultEnum, Aircraft>(PredictorResultEnum.Hide, null);
			}

			//Odfiltrované redundantní fixy.
			var filteredFixes = this.RemoveRedundantFixes(rawData, currentFixDateTime);

			//vylistuje všechny stavové vektory.
			var allStateVectors = rawData.ListAllStateVectors();

			//Určí kurz.
			var track = currentStateVector.Track ?? 0;
			currentStateVector.Track = track;

			//Určí rychlost letadla.
			var groundSpeed = this.GetGroundSpeed(currentStateVector.GroundSpeed);
			currentStateVector.GroundSpeed = groundSpeed;

			//Určí zda letadlo letí nebo je na zemi.
			var isOnGround = this.IsOnGround(currentStateVector, airportArea);
			currentStateVector.OnGround = isOnGround;

			//Aktuální poloha letadla. Pokud nelze dopočítat polohu letadla (např. máme jen jeden fix), použijeme poslední reálný fix.
			var currentFix = this.GetLocation(currentFixDateTime, allStateVectors) ?? allStateVectors.Last().RealFix;

			if (!this.monitoredArea.Contains(currentFix.Location)) {
				//Letadlo se nachazí mimo sledovanou oblast -> je odstraněno.
				return new Tuple<PredictorResultEnum, Aircraft>(PredictorResultEnum.Remove, null);
			}

			//Určí rychlost stoupání/klesání.
			var climbRate = this.DetermineVerticalSpeed(currentFixDateTime, currentFix, filteredFixes);
			currentStateVector.VerticalSpeed = climbRate;

			//Manévr letadla.
			currentStateVector.Maneuver = this.DetermineManeuver(climbRate);

			//Ukazatel směru a rychlosti (čára před cílem).
			var speedVectorEndPoint = this.Extrapolate(currentFix.DateTime.AddSeconds(this.SpeedVectorTimeLength), filteredFixes, false);

			//Historické polohy.
			var trailDots = new List<AircraftFix>();
			foreach (var trailDotTime in trailDotsDateTime) {
				var trailDot = this.GetLocation(trailDotTime, filteredFixes);
				if (!trailDot.HasValue) {
					continue;
				}

				//Pouze projistotu, pokud by náhodou z důvodu různých časů vykreslení se trail dot dostal před aktuální polohu.
				if (trailDot.Value.DateTime < currentFix.DateTime) {
					trailDots.Add(trailDot.Value);
				}
			}

			//Vloží predikovanou polohu do nového objektu.
			var predictedStateVector = new AircraftPredictedStateVector(currentStateVector, currentFix);
			var predictedAircraft = new Aircraft(rawData.Id, predictedStateVector, trailDots, speedVectorEndPoint, filteredFixes.Select(x => x.RealFix).ToList(), rawData.Info, rawData.ListParticipatingDataSources());

			return new Tuple<PredictorResultEnum, Aircraft>(PredictorResultEnum.Show, predictedAircraft);
		}

		/// <summary>
		/// Vrací polohu pro zadaný čas.
		/// </summary>
		/// <param name="dateTime">Čas, pro který se hledá poloha.</param>
		/// <param name="stateVectors">Stavové vektory z datových zdrojů.</param>
		/// <returns>Extrapolovaná nebo interpolovaná poloha.</returns>
		private AircraftFix? GetLocation(DateTime dateTime, List<AircraftStateVector> stateVectors)
		{
			//Určí jestli se hledaný fix nachází v budoucnosti nebo minulosti.
			if (dateTime > stateVectors.Last().RealFix.DateTime) {
				//Budoucnost (nemáme k dispozici reálné fixy) => Extrapolujeme.
				return this.Extrapolate(dateTime, stateVectors);
			} else {
				// máme k dispozici reálné fixy a hledaný fix je mladší nebo rovný nejstaršímu reálnému fixu => Interpolujeme.
				return this.Interpolate(dateTime, stateVectors);
			}
		}

		/// <summary>
		/// Interpoluje polohu pomocí linární interpolace.
		/// </summary>
		/// <param name="dateTime">Čas, pro který hledáme polohu.</param>
		/// <param name="stateVectors">Stavové vektory z datových zxrojů.</param>
		/// <returns>Interpolovaný fix.</returns>
		private AircraftFix? Interpolate(DateTime dateTime, List<AircraftStateVector> stateVectors)
		{
			//Najde fixy mezi kterýými se bude interpolovat.
			var fixes = this.FindNeighbors(stateVectors.Select(x => x.RealFix).ToList(), dateTime);
			if (fixes == null) {
				return null;
			}

			//Interpoluje pro každou souřadnice prostoru.
			var latitude = MathExtension.LinearInterpolateInBoundlessInterval(fixes[0].Location.Latitude, fixes[1].Location.Latitude, fixes[0].DateTime, fixes[1].DateTime, dateTime);
			var longitude = MathExtension.LinearInterpolateInBoundlessInterval(fixes[0].Location.Longitude, fixes[1].Location.Longitude, fixes[0].DateTime, fixes[1].DateTime, dateTime);
			var altitude = MathExtension.LinearInterpolateInBoundlessInterval(fixes[0].Altitude, fixes[1].Altitude, fixes[0].DateTime, fixes[1].DateTime, dateTime);

			return new AircraftFix(new Location(latitude, longitude), altitude, dateTime);
		}

		/// <summary>
		/// Najde nebližší reálně fixy z datových zdrojů.
		/// </summary>
		/// <param name="realFixes">Reálné fixy z datových zdrojů.</param>
		/// <param name="originDateTime">čas fixu, pro který jsou sousendí fixy hledány.</param>
		/// <returns>Pole sousedů. Může obsahovat pouze jednoho souseda, pokud druhý není k dispozici.</returns>
		private AircraftFix[] FindNeighbors(List<AircraftFix> realFixes, DateTime originDateTime)
		{
			var first = realFixes.LastOrDefault(x => x.DateTime < originDateTime);
			var second = realFixes.FirstOrDefault(x => x.DateTime >= originDateTime);
			if (!first.Equals(default(AircraftFix)) && !second.Equals(default(AircraftFix))) {
				return new AircraftFix[] {first, second};
			}

			return null;
		}

		/// <summary>
		/// Extrapoluje (predikuje) polohu letadla.
		/// </summary>
		/// <param name="timeForPrediction">Čas pro který bude predikovaná poloha.</param>
		/// <param name="stateVectors">Stavové vektory letadla, na základe který bude poloha extrapolována.</param>
		/// <param name="enabledPredictionTimeLimit">Hodnota, která určuje zda je nastavený limit pro predikci.</param>
		/// <returns>Predikovaná poloha.</returns>
		private AircraftFix Extrapolate(DateTime timeForPrediction, List<AircraftStateVector> stateVectors, bool enabledPredictionTimeLimit = true)
		{
			if (enabledPredictionTimeLimit) {
				//Opraví čas predikce, pokud už predikujeme moc dopředu.
				timeForPrediction = this.CorrectTimeForPrediction(timeForPrediction, stateVectors);
			}

			var x = new List<double>();
			var y = new List<double>();
			var z = new List<double>();
			var t = new List<double>();

			//Hledá fixy za posledních MinFixTime sekund, které budou požité pro extrapolaci.
			var realFixes = stateVectors.Select(f => f.RealFix).ToList();
			for (var i = realFixes.Count - 1; i >= 0; i--) {
				if ((realFixes.Last().DateTime - realFixes[i].DateTime).TotalSeconds >= this.MinTimeIntervalForPrediction) {
					break;
				}

				x.Add(realFixes[i].Location.Longitude);
				y.Add(realFixes[i].Location.Latitude);
				z.Add(realFixes[i].Altitude);

				//Převede na časy, které budou představovat osu x.
				t.Add((timeForPrediction - realFixes[i].DateTime).TotalSeconds);
			}

			var tArray = t.ToArray();
			var xArray = x.ToArray();
			var yArray = y.ToArray();
			var zArray = z.ToArray();


			//Pokud je počet nalezených fixů více než konstanta MinFixCountForLinearRegression => extrapoluj pomocí proložení bodů přímkou
			if (t.Count >= this.MinFixCountForLinearRegresion) {

				//Pro nadmořskou výšku využíváme pouze proložení bodů přímkou.
				var predictedAltitude = (float)this.FitLine(tArray, zArray);

				//Predikovaná poloha.
				var predictedLocation = this.ExtrapolateByLineOrCircle(tArray, xArray, yArray);

				return new AircraftFix(new Location((float)predictedLocation.Item2, (float)predictedLocation.Item1), predictedAltitude, timeForPrediction);
			}

			//Nemáme dost fixu, vrátíme poslední reálný fix.
			//Tento stav bude nastávat jen při spuštění aplikace => po zhruba 15 vteřinách už bude extrapolace fungovat.
			return stateVectors.Last().RealFix;
		}

		/// <summary>
		/// Určí křivku (kružnice a přímka) nejvohdnější pro extrapolaci. Vhodnější křivku použije pro extrapolaci.
		/// </summary>
		/// <param name="t">Časy reálných fixů.</param>
		/// <param name="x">Zeměpisné délky reálných fixů.</param>
		/// <param name="y">Zeměpisné šířky reálných fixů.</param>
		/// <returns>Predikovaná poloha.</returns>
		private Tuple<double, double> ExtrapolateByLineOrCircle(double[] t, double[] x, double[] y)
		{
			var points = new List<Point2D>();
			for (var i = 0; i < x.Length; i++) {
				points.Add(new Point2D(x[i], y[i]));
			}

			//Nafituje kružnici.
			var circleFittingResult = MathFittingCircle.FitCircle(points);
			Debug.WriteLine(circleFittingResult.Radius);

			if (circleFittingResult.Radius < this.MaxCircleRadiusForFitting) {
				//Kružnice má poloměr menší než je stanovená hodnota
				var firstAngle = Math.Atan2(y[0] - circleFittingResult.Center.Y, x[0] - circleFittingResult.Center.X);
				var secondAngle = Math.Atan2(y[1] - circleFittingResult.Center.Y, x[1] - circleFittingResult.Center.X);

				//Určí uhlovou rychlost.
				var angularSpeed = (secondAngle - firstAngle) / (t[1] - t[0]);

				//Určí poloměr + odchylka nejnovějšího fixu.
				var radius = circleFittingResult.Radius;
				radius += circleFittingResult.Distances[0];

				var point = GetPointOnCircle(radius, firstAngle - angularSpeed * t[0], circleFittingResult.Center);
				return new Tuple<double, double>(point.X, point.Y);
			}


			//Proloží body přímkou.
			var lineX = Fit.Line(t, x);
			var lineY = Fit.Line(t, y);

			//Nemůžeme nafitovat kužnici nebo kružnice je moc velká nebo kvalita fitu je u kružnice menší => použijeme proložení přímkou
			return new Tuple<double, double>(lineX.Item1, lineY.Item1);

		}

		/// <summary>
		/// Najde bod na kružnici.
		/// </summary>
		/// <param name="radius">Poloměr kružnice.</param>
		/// <param name="angleRadian">Úhel v radiánech.</param>
		/// <param name="origin">Střed kružnice.</param>
		/// <returns>Bod, který se nachází na kružnici pod zadaným úhlem.</returns>
		public static Point2D GetPointOnCircle(double radius, double angleRadian, Point2D origin)
		{     
			var x = radius * Math.Cos(angleRadian) + origin.X;
			var y = radius * Math.Sin(angleRadian) + origin.Y;

			return new Point2D(x, y);
		}

		/// <summary>
		/// Určí čas predikce. Pokud je čas větší než stanovené maximum (predikujeme už moc dopředu), vrátí poslední možný čas predikce.
		/// </summary>
		/// <param name="timeForPrediction">standartní čas predikce.</param>
		/// <param name="stateVectors">stavové vektory z datových zdrojů.</param>
		/// <returns>Opravený čas predikce.</returns>
		private DateTime CorrectTimeForPrediction(DateTime timeForPrediction, List<AircraftStateVector> stateVectors)
		{
			var lastRealFix = stateVectors.Last().RealFix.DateTime;
			var timeBetweenLastRealFixAndPredicatedFix = (timeForPrediction - lastRealFix).TotalSeconds;
			if (timeBetweenLastRealFixAndPredicatedFix > this.MaxPredictedSeconds) {

				//Počet sekund od posledního fixu překročil hranici = predikujeme už moc dopředu => predikce se zastaví.
				timeForPrediction = lastRealFix.AddSeconds(this.MaxPredictedSeconds);
			}

			return timeForPrediction;
		}

		/// <summary>
		/// Proloží body přímkou a vrátí predikovaný bod pro čas 0.
		/// Časy musí být takové, aby čas hledaného (predikovaného) fixu byla 0.
		/// přimka je pak dána y: x => a;
		/// </summary>
		/// <param name="t">pole časových parametrů funkce.</param>
		/// <param name="x">pole hodnot funkce.</param>
		/// <returns>Predikovaný bod.</returns>
		private double FitLine(double[] t, double[] x)
		{
			var result = Fit.Line(t, x);
			return result.Item1;
		}

		//-------------------------------------------------------------------------------------------------------------------------------
		//        SPEED VECTOR -> v aktuální verzi nepoužíváme, neboť kurz, který dostáváme z datových zdrojů není spolehlivý
		//-------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Najde konec rychlostního vektoru (speed vektor).
		/// </summary>
		/// <param name="currentFix">Aktuální fix.</param>
		/// <param name="speedVector">Speed vector.</param>
		/// <param name="timeLength">Časov určující délku speed vectoru.</param>
		/// <returns>Fix určující konec speed vectoru.</returns>
		private AircraftFix FindSpeedVectorEndFix(AircraftFix currentFix, Vector3 speedVector, int timeLength)
		{
			var distance = speedVector * timeLength;
			var altitude = currentFix.Altitude + distance.Z;
			var location = Location.TranslateLocation(currentFix.Location, new Vector2(distance.X, distance.Y));

			return new AircraftFix(location, altitude, currentFix.DateTime.AddSeconds(timeLength));
		}

		/// <summary>
		/// Určí rychlostní vektor (speed vector) letadla.
		/// </summary>
		/// <param name="speed">Rychlost letadla (m/s)</param>
		/// <param name="angle">Kurz. (°).</param>
		/// <param name="verticalSpeed">Vertikální rychlost. (m/s).</param>
		/// <returns>Směrový vektor.</returns>
		private Vector3 GetSpeedVector(float speed, double angle, float verticalSpeed)
		{
			var x = speed * Math.Sin(MathExtension.DegreeToRadian(angle));
			var y = speed * Math.Cos(MathExtension.DegreeToRadian(angle));
			return new Vector3((float) x, (float) y, verticalSpeed);
		}

		//-------------------------------------------------------------------------------------------------------------------------------
		//                                                  MANUVER
		//-------------------------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Určí rychlost stoupání/klesání (m/s).
		/// Vypočet probíhá na základě rozdílů výšek mezi fixy.
		/// </summary>
		/// <returns>Rychlost stoupání/klesání.</returns>
		private float DetermineVerticalSpeed(DateTime predictionTime, AircraftFix predictedFix, List<AircraftStateVector> stateVectors)
		{
			//Najdeme druhý fix, který se nachází v historii. Fixy mají rozmezí časů daných VerticalSpeedTimeDiff.
			var secondFix = this.GetLocation(predictionTime.AddSeconds(-this.VerticalSpeedTimeDiff), stateVectors);
			if (secondFix == null) {
				return 0;
			}

			var timeDiff = predictedFix.DateTime - secondFix.Value.DateTime;
			var altitudeDiff = predictedFix.Altitude - secondFix.Value.Altitude;

			if (timeDiff.TotalSeconds <= 0) {
				//Přišla nová informace o letadle. Rychost stoupání/klesání je nastavena na 0.
				return 0;
			}

			var climbRate = altitudeDiff / (float)timeDiff.TotalSeconds;
			if (Math.Abs(climbRate) <= this.VerticalSpeedThreshold) {
				return 0;
			}

			return climbRate;
		}

		//-------------------------------------------------------------------------------------------------------------------------------
		//-------------------------------------------------------------------------------------------------------------------------------


		/// <summary>
		/// Určí aktuální manévr, který letadlo provádí.
		/// </summary>
		/// <param name="climbRate">Rychlost stoupání/klesání.</param>
		/// <returns>Typ manévru.</returns>
		private AircraftManeuver DetermineManeuver(float climbRate)
		{
			if (climbRate > 0) {
				//letadlo stoupá.
				return AircraftManeuver.Climb;
			} else if (climbRate < 0) {
				//letadlo klesá.
				return AircraftManeuver.Descent;
			} else {
				//letadlo udržuje výšku.
				return AircraftManeuver.Horizon;
			}
		}

		//-------------------------------------------------------------------------------------------------------------------------------
		//-------------------------------------------------------------------------------------------------------------------------------


		/// <summary>
		/// Určí zda je letadlo letí nebo je letadlo na zemi (již přistálo).
		/// </summary>
		/// <param name="currentStateVector">Aktuální stavový vektor letadla.</param>
		/// <param name="airportArea">Oblast kolem letiště, která slouží k detekci letadel na zemi.</param>
		/// <returns>true pokud je letadlo na zemi, jinak false.</returns>
		private bool IsOnGround(AircraftStateVector currentStateVector, Tuple<int, BoundingBox> airportArea)
		{
			if (currentStateVector.OnGround.HasValue) {
				//Je k dispozici infomace o tom, zda je letadlo na zemi => použijeme ji.
				return currentStateVector.OnGround.Value;
			}

			var altitudeInMeters = MathExtension.FeetToMeters(airportArea.Item1);
			var boundingBox = airportArea.Item2;

			//Rozdíl výšky letadla a letiště.
			var altitudeDiff = currentStateVector.RealFix.Altitude - altitudeInMeters;

			if (boundingBox.Contains(currentStateVector.RealFix.Location) && 
			    altitudeDiff <= this.AltitudeThresholdForOnGroundDetection
			    && currentStateVector.GroundSpeed <= this.MaxSpeedForOnGroundDetection) {

				//Pokud se letadlo nachází v oblasti letiště, jeho výška odpovídá výšce letiště (s odchylkou) a rychlost je dostatečně malá -> prohlášeno, že letadlo je na zemi.
				return true;
			}

			return false;
		}


		/// <summary>
		/// Určí rychlost letadla v metrech za sekundu.
		/// </summary>
		/// <param name="groundSpeed">Traťový rychlost.</param>
		/// <returns>Rychlost letadla.</returns>
		private float GetGroundSpeed(float? groundSpeed)
		{
			//Pokud není k dispozici informace o rychlost nastaví rychlost na 0.
			return groundSpeed ?? 0;
		}

		/// <summary>
		/// Odstraní redundatní fixy.
		/// Vyběr fixů probíhá na základě spolehlivosti jednotlivých datových zdrojů.
		/// Pokud je dostatek reálných fixů ze nejspolehlivějšího datového zdroje, použijí se ty.
		/// V případě, že pro určitý časový interval nebyl přijat reálný fix z tohoto zdroje, je doplněn fixy z dalšího datového zdroje.
		/// </summary>
		/// <returns>Seznam neobsahující redundantní fixy.</returns>
		private List<AircraftStateVector> RemoveRedundantFixes(AircraftRawData rawData, DateTime predictionTime)
		{
			var diff = 10;
			var allStateVectors = rawData.ListAllStateVectors();
			var result = new Stack<AircraftStateVector>();

			var indexOfYoungestRealFix = this.GetIndexOfYoungestRealFixAffectedByPriority(allStateVectors, predictionTime);

			var youngestStateVector = allStateVectors[indexOfYoungestRealFix];
			result.Push(youngestStateVector);

			var i = indexOfYoungestRealFix;
			while (i >= 1) {

				var j = i - 1;
				var maxPriority = int.MaxValue;
				var indexOfMaxPriority = j;

				while (j >= 0) {

					var previousStateVector = allStateVectors[j];
					var previousStateVectorPriority = DataSourcePriorityHelper.GetPriorityByDataSource(previousStateVector.DataSource);

					if ((allStateVectors[i].RealFix.DateTime - previousStateVector.RealFix.DateTime).TotalSeconds > diff) {
						break;
					}

					if (previousStateVectorPriority < maxPriority) {
						maxPriority = previousStateVectorPriority;
						indexOfMaxPriority = j;
					}

					j--;
				}

				result.Push(allStateVectors[indexOfMaxPriority]);
				i = indexOfMaxPriority;
			}

			return result.ToList();
		}

		/// <summary>
		/// Najde index nejnovějšího reálného fixu, kdy bereme v potaz prioritu datového zdroje.
		/// </summary>
		/// <param name="allStateVectors">Stavové vektory letadel.</param>
		/// <param name="predictionTime">Čas predikce.</param>
		/// <returns>Index nejnovějšího reálného fixu.</returns>
		private int GetIndexOfYoungestRealFixAffectedByPriority(List<AircraftStateVector> allStateVectors, DateTime predictionTime)
		{
			var diff = 10;

			var i = allStateVectors.Count - 1;
			var maxPriority = int.MaxValue;
			var indexOfMaxPriority = i;

			while (i >= 0 && (predictionTime - allStateVectors[i].RealFix.DateTime).TotalSeconds <= diff) {
				var priority = DataSourcePriorityHelper.GetPriorityByDataSource(allStateVectors[i].DataSource);
				if (priority < maxPriority) {
					maxPriority = priority;
					indexOfMaxPriority = i;
				}

				i--;
			}

			return indexOfMaxPriority;
		}
	}
}