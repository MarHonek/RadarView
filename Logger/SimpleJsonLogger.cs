using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Logger
{
    /// <summary>
    /// Trida pro logovani dat do souboru ve formatu json.
    /// </summary>
    /// <typeparam name="T">Typ dat</typeparam>
    public class SimpleJsonLogger<T> : Logger
    {
        /// <summary>
        /// Cas (hodina) urcujici do jakeho souboru se logy budou zapisovat.
        /// Napr. Pro 15:30 se budou logy zapisovat do souboru s nazvem g
        /// </summary>
        private DateTime currentWriteFileDateTime;


        /// <summary>
        /// StreamWriter pro zapisování logu do souboru.
        /// </summary>
        private StreamWriter streamWriter;
        

        /// <summary>
        /// StreamReader pro načtení logu ze souboru při replay.
        /// </summary>
        private StreamReader streamReader;


        /// <summary>
        /// Event vyvolán pokud je načten a zpracován jednotlivý log při replay.
        /// </summary>
        public event EventHandler<JsonLogEventArgs<T>> LogItemReplayed;


        /// <summary>
        /// Formát názvu souboru pro uložení logu. Formát HH odpovídá hodině (UTC), např. 16 = 16:00
        /// </summary>
        private const string FILE_NAME_FORMAT = "HH";


        /// <summary>
        /// Přípona souboru.
        /// </summary>
        private const string FILE_EXTENSION = ".txt";


        /// <summary>
        /// Časovač pro zpětné přehrávání (Replay).
        /// Čas mezi tickami časovače odpovídají rozdílům času uložených logů.
        /// </summary>
        private System.Timers.Timer replayTimer;


        /// <summary>
        /// Seznam deserializovaných logů.
        /// </summary>
        private List<JsonLog<T>> deserializedLogs;


        /// <summary>
        /// Číslo řádků v seznamu deserializovaných logů.
        /// </summary>
        private int replayCounter;


        /// <summary>
        /// Aktuálně načtené logy.
        /// </summary>
        private DateTime currentReadFileDateTime;


        /// <summary>
        /// Slovník obsahující deserializovane logy. Klíč je časová známka uložení logu. Hodnotou jsou data typu T.
        /// </summary>
        private Dictionary<long, T> deserializedLogsDict;


		/// <summary>
		/// Event vyvolán pokud přehrávání skončilo.
		/// </summary>
		public event EventHandler ReplayIsOver;

        /// <summary>
        /// Vytvoří novou instanci třídy SimpleJsonLogger.
        /// </summary>
        /// <param name="path">Root cesta ke složce, kam se mají logy ukládat.</param>
        public SimpleJsonLogger(string path) : base(path)
        {
        }

        /// <summary>
        /// Inicializuje timer pro zpětné přehrávání (replay) z logů.
        /// </summary>
        private void InitReplayTimer()
        {
            this.replayTimer = new System.Timers.Timer();
            this.replayTimer.Elapsed += this.ReplayTimer_Elapsed;
            this.replayTimer.Interval = 1;
        }


        private void ReplayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
	        //deserializuje json
	        var jsonLog = this.deserializedLogs[this.replayCounter];

	        TimeSpan timeDiff;

	        if (this.replayCounter >= this.deserializedLogs.Count - 1) {
		        this.StopReplaying();
		        this.ReplayIsOver?.Invoke(this, new EventArgs());
	        } else {
		        timeDiff = this.deserializedLogs[this.replayCounter + 1].LogDate - this.deserializedLogs[this.replayCounter].LogDate;
		        var timer = sender as System.Timers.Timer;
		        if (Math.Abs(timeDiff.TotalMilliseconds) < 0.0001) {
			        timer.Interval = 1;
		        } else {
			        timer.Interval = timeDiff.TotalMilliseconds;
		        }
	        }

	        Interlocked.Increment(ref this.replayCounter);
	        this.LogItemReplayed?.Invoke(this, new JsonLogEventArgs<T>(jsonLog));
        }

        /// <summary>
        /// Získá a vrátí log pro zadaný čas.
        /// </summary>
        /// <param name="timestamp">čas logu.</param>
        /// <returns>Log pro zadaný čas, jestliže neexistuje vrátí null.</returns>
        public T GetLog(long timestamp)
        {
	        var dateTimeFromTimestamp = DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;

	        if (dateTimeFromTimestamp.Date != this.currentReadFileDateTime.Date && dateTimeFromTimestamp.Hour != this.currentReadFileDateTime.Hour) {
		        //Požadovaný log není v již načteném souboru => načte a deserializuje logy z nového souboru.
		        var filePath = this.GetPathOrNull(dateTimeFromTimestamp);
		        this.deserializedLogsDict = new Dictionary<long, T>();
		        if (filePath != null) {
			        using (var reader = new StreamReader(filePath)) {
				        //Načte soubor a deserializuje jednotlivé logy z formátu JSON.
				        while (!reader.EndOfStream) {
					        var line = reader.ReadLine();
					        var deserializedLog = JsonConvert.DeserializeObject<JsonLog<T>>(line);
					        this.deserializedLogsDict[deserializedLog.LogTimeStamp] = deserializedLog.Data;
				        }
			        }
		        }

		        //Nastaví aktuální čas => logy pro stejnou hodinu se budou načítat z kolekce tj. není potřeba je načítat ze souboru.
		        this.currentReadFileDateTime = dateTimeFromTimestamp;
	        }

	        var log = this.deserializedLogsDict[timestamp];
	        return log;
        }

        /// <summary>
        /// Získá cestu k souboru podle zadaného času.
        /// </summary>
        /// <param name="dateTime">čas odpovídající hledanému souboru.</param>
        /// <returns>cestu k souboru pokud existuje, jinak false.</returns>
        private string GetPathOrNull(DateTime dateTime)
        {
	        if (!this.FileExists(dateTime)) {
		        return null;
	        }

	        return this.GetFilePath(dateTime);
        }

        /// <summary>
        /// Zapne zpětné přehrávání logů ze zadaného souboru 
        /// </summary>
        /// <param name="externalFileName">Název souboru s logy. Celá cesta k souboru je dána parametrem Path vloženeho do konstruktoru.</param>
        /// <returns>Časový posun informací vůči aktuálnímu času.</returns>
        public TimeSpan StartReplaying(string externalFileName)
        {
	        var externalFileExists = File.Exists(Path.Combine(this.RootPath, externalFileName));
	        if (!externalFileExists) {
		        //Soubor neexistuje.
		        throw new FileForReplayDoNotExistsException("Soubor s logy pro zpětné přehrávání neexistuje.");
	        }

	        var deserializedLogs = new List<JsonLog<T>>();
	        using (var streamReader = new StreamReader(Path.Combine(this.RootPath, externalFileName))) {

		        //Načte všechny logy ze souboru a deserializuje je.
		        while (!streamReader.EndOfStream) {
			        deserializedLogs.Add(JsonConvert.DeserializeObject<JsonLog<T>>(streamReader.ReadLine()));
		        }
	        }

	        if (deserializedLogs.Count > 0) {

		        this.deserializedLogs = deserializedLogs;

		        //Inicializuje a spustí časovač.
		        this.InitReplayTimer();
		        this.replayTimer.Start();
	        }

	        var utcNow = DateTime.UtcNow;
	        if (deserializedLogs.Count >= 0) {
		        return utcNow - deserializedLogs.FirstOrDefault().LogDate;
			}

	        return new TimeSpan(0,0,0,0);
        }

        /// <summary>
        /// Zastaví časovač pro zpětné přehrávání (Replay).
        /// </summary>
        public void StopReplaying()
        {
            this.replayTimer.Stop();
            this.replayCounter = 0;
            this.deserializedLogs.Clear();
        }

        /// <summary>
        /// Serializuje data do formátu JSON, který uloží do souboru odpovídající aktuální hodině (UTC).
        /// </summary>
        /// <param name="data">Data.</param>
        public void Write(T data)
        {
	        if (this.streamReader != null) {
		        throw new InvalidOperationException("Neni ukonceno cteni ze souboru. Pred zapisovanim zavolej metodu CloseReading.");
	        }

	        var dateTimeUtcNow = DateTime.UtcNow;

	        if (this.streamWriter == null || !this.LogHasSameFilePath(dateTimeUtcNow)) {
		        this.streamWriter = this.ChangeLogFile(dateTimeUtcNow);
	        }

	        var jsonLog = new JsonLog<T>(dateTimeUtcNow, data);
	        var logInJson = JsonConvert.SerializeObject(jsonLog);
	        this.streamWriter.WriteLine(logInJson);
	        this.streamWriter.Flush();
        }

        /// <summary>
        /// Vytvoří/otevře soubor pro zapisování logu v případě že se změnil čas nebo datum.
        /// </summary>
        /// <param name="dateTime">Datum a čas logu.</param>
        /// <returns>Nová instance třídy StreamWriter pro zápis do souboru.</returns>
        private StreamWriter ChangeLogFile(DateTime dateTime)
        {
	        this.SetFolderPathIfNewDayHasCome(dateTime);
	        this.streamWriter?.Close();

	        this.currentWriteFileDateTime = dateTime;
	        return new StreamWriter(this.GetFilePath(dateTime), true);
        }

        /// <summary>
        /// Určí zda log pro zadané datum a čas má být uložen do aktuálně otevřeného souboru.
        /// </summary>
        /// <param name="dateTime">Čas logu.</param>
        /// <returns>true pokud log patří do aktuálně otevřeného souboru, jinak false.</returns>
        private bool LogHasSameFilePath(DateTime dateTime)
        {
            return this.currentWriteFileDateTime.Date == dateTime.Date && this.currentWriteFileDateTime.Hour == dateTime.Hour;
        }

        /// <summary>
        /// Vraca cestu k souboru podle zadanaho data a casu.
        /// </summary>
        /// <param name="dateTime">datum a cas pro urceni souboru.</param>
        /// <returns>Cestu k souboru. Soubor ani cesta k nemu nemusi existovat.</returns>
        public string GetFilePath(DateTime dateTime)
        {
            return Path.ChangeExtension(Path.Combine(this.GetFolderPathByDate(dateTime), dateTime.ToString(FILE_NAME_FORMAT)), FILE_EXTENSION);
        }

        /// <summary>
        /// Určí zda soubor pro zadaný čas existuje.
        /// </summary>
        /// <param name="dateTime">čas logu uložený v hledaném souboru.</param>
        /// <returns>true pokud soubor existuje, jinak false.</returns>
        public bool FileExists(DateTime dateTime)
        {                     
            return File.Exists(this.GetFilePath(dateTime));
        }

        /// <summary>
        /// Zavre streamy pro zapis i cteni ze souboru.
        /// </summary>
        public void Close()
        {
            this.CloseReading();
            this.CloseWriting();
        }

        /// <summary>
        /// Zavre stream pro zapis do souboru.
        /// </summary>
        public void CloseWriting()
        {
            this.streamWriter?.Close();
            this.streamWriter = null;
        }

        /// <summary>
        /// Zavre stream pro cteni ze souboru.
        /// </summary>
        public void CloseReading()
        {
            this.streamReader?.Close();
            this.streamReader = null;
        }
    }

    /// <summary>
    /// Třída reprezentuje log.
    /// Obsahuje data a čas uložení logu ve formátu json.
    /// </summary>
    /// <typeparam name="T">Typ dat.</typeparam>
    public class JsonLog<T>
    {
        /// <summary>
        /// Datum a čas zalogování.
        /// </summary>
        [JsonIgnore]
        public DateTime LogDate { get; set; }

        /// <summary>
        /// Časová známka odpovídající datu a času zalogování.
        /// </summary>
        private long _logTimeStamp;

        public long LogTimeStamp
        {
	        set
	        {
		        this._logTimeStamp = value;
		        this.LogDate = DateTimeOffset.FromUnixTimeSeconds(value).DateTime;
	        }
	        get
	        {
		        var dateTimeOffset = new DateTimeOffset(this.LogDate.ToLocalTime());
		        return dateTimeOffset.ToUnixTimeSeconds();
	        }
        }

        /// <summary>
        /// Libovolná datová struktura obsahujicí data k zalogování.
        /// </summary>
        public T Data { get; set; }

        [JsonConstructor]
        public JsonLog(long logTimeStamp, T data)
        {
            this.LogTimeStamp = logTimeStamp;
            this.Data = data;
        }

        /// <summary>
        /// Vytvoří novou instanci třídy JsonLog.
        /// </summary>
        /// <param name="logDate">datum zalogování.</param>
        /// <param name="data">libovolná data k zalogování.</param>
        public JsonLog(DateTime logDate, T data)
        {
            this.LogDate = logDate;
            this.Data = data;
        }
    }

    /// <summary>
    /// Argumenty události vyvolané při zpětném přehrávání.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonLogEventArgs<T> : JsonLog<T>
    {
        public JsonLogEventArgs(JsonLog<T> jsonLog) : base(jsonLog.LogDate, jsonLog.Data)
        { }
    }
}
