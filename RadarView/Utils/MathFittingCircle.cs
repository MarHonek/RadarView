using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using SharpDX;
using Matrix = MathNet.Numerics.LinearAlgebra.Double.Matrix;

namespace RadarView.Utils
{
	/// <summary>
	/// Třída, která se stará o proložení bodů kružnicí.
	/// Kód přepsán z: https://github.com/Meakk/circle-fit/blob/master/circlefit.js
	/// </summary>
	public static class MathFittingCircle
	{
		/// <summary>
		/// Proloží body kružnicí.
		/// </summary>
		/// <param name="points">Seznam bodů.</param>
		/// <returns>Objekt, který v sobě obsahuje výsledky proložení bodů kružnicí.</returns>
		public static FittingCircleResult FitCircle(List<Point2D> points)
		{
			/*for (int i = 0; i < points.Count; i++) {
				points[i].X *= 10000;
				points[i].Y *= 1000000000;
			}*/
			var result = new FittingCircleResult();

			//zprůměruje.
			var m = DetermineMeans(points);

			var pointsP = new List<Point2D>();
			for (var i = 0; i < points.Count; i++) {
				pointsP.Add(new Point2D(points[i].X - m.X, points[i].Y - m.Y));
			}

			double sxx = 0;
			double sxy = 0;
			double syy = 0;

			double vectorX = 0;
			double vectorY = 0;

			//Vyřeší lineární rovnici.
			for (var i = 0; i < pointsP.Count; i++) {
				sxx += pointsP[i].X * pointsP[i].X;
				sxy += pointsP[i].X * pointsP[i].Y;
				syy += pointsP[i].Y * pointsP[i].Y;

				vectorX += 0.5 * (pointsP[i].X * pointsP[i].X * pointsP[i].X + pointsP[i].X * pointsP[i].Y * pointsP[i].Y);
				vectorY += 0.5 * (pointsP[i].Y * pointsP[i].Y * pointsP[i].Y + pointsP[i].X * pointsP[i].X * pointsP[i].Y);
			}

			Matrix<double> matrix = Matrix.Build.Dense(2, 2);
			matrix[0, 0] = sxx;
			matrix[0, 1] = sxy;
			matrix[1, 0] = sxy;
			matrix[1, 1] = syy;

			var vector = new DenseVector(2);
			vector[0] = vectorX;
			vector[1] = vectorY;
			var solution = Solve(matrix, vector);


			if (solution == null) {
				//Nemáme dost bodů nebo jsou body kolineární.
				return null;
			}

			//Určí poloměr kružnice
			var radius2 = solution.X * solution.X + solution.Y * solution.Y + (sxx + syy) / points.Count;
			result.Radius = Math.Sqrt(radius2);

			result.Center = new Point2D(solution.X + m.X, solution.Y + m.Y);

			result.Distances = new List<double>();
			result.Projection = new List<Point2D>();

			foreach (var point2D in points) {
				var v = new Point2D(point2D.X - result.Center.X, point2D.Y - result.Center.Y);
				var len2 = v.X * v.X + v.Y * v.Y;
				result.Residue += radius2 - len2;
				result.SumOfSquaredErrors += Math.Abs(radius2 - len2);
				var len = Math.Sqrt(len2);
				result.Distances.Add(len - result.Radius);
				result.Projection.Add(new Point2D(result.Center.X + v.X * result.Radius / len, result.Center.Y + v.Y * result.Radius / len));

			}

			return result;
		}

		/// <summary>
		/// Vyřeší rovnice.
		/// </summary>
		private static Point2D Solve(Matrix<double> matrix, DenseVector vector)
		{
			var determinant = matrix.Determinant();
			/*if (determinant < 1e-8 && determinant > -1e-8) {
				return null; //nemá řešení.
			}*/

			var y = matrix[0, 0] * vector[1] - matrix[1, 0] * vector[0] / determinant;
			var x = (vector[0] - matrix[0, 1] * y) / matrix[0, 0];
			return new Point2D(x.Real, y.Real);
		}

		/// <summary>
		/// Zprůměruje.
		/// </summary>
		private static Point2D DetermineMeans(List<Point2D> points)
		{
			var result = new Point2D(0, 0);
			for (var i = 0; i < points.Count; i++) {
				result.X += points[i].X / points.Count;
				result.Y += points[i].Y / points.Count;
			}

			return result;
		}
	}

	/// <summary>
	/// Pomocná třída pro reprezentaci bodu na ploše.
	/// </summary>
	public class Point2D
	{
		/// <summary>
		/// X souřadnice.
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Y souřadnice.
		/// </summary>
		public double Y { get; set; }

		public Point2D(double x, double y)
		{
			this.X = x;
			this.Y = y;
		}
	}

	/// <summary>
	/// Wrapper pro výsledky proložení bodů kružnicí.
	/// </summary>
	public class FittingCircleResult
	{
		/// <summary>
		/// Poloměr kružnice.
		/// </summary>
		public double Radius { get; set; }

		/// <summary>
		/// Střed kružnice.
		/// </summary>
		public Point2D Center { get; set; }

		/// <summary>
		/// Zbytek.
		/// </summary>
		public double Residue { get; set; }

		/// <summary>
		/// Součet odchylek r^2 od modelů.
		/// </summary>
		public double SumOfSquaredErrors { get; set; }

		/// <summary>
		/// Vzdálenosti.
		/// </summary>
		public List<double> Distances { get; set; }

		/// <summary>
		/// Projekce.
		/// </summary>
		public List<Point2D> Projection { get; set; }
	}
}