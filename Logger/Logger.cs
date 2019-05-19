using System;
using System.IO;

namespace Logger
{
    /// <summary>
    /// Abstraktni trida starajici se o logovani zprav.
    /// V teto tride jsou implementovane zakladni metody pro vytvoreni slozek a spravne umistovani log souboru do nich.
    /// Logy jsou rozdeleny do souboru podle aktualni hodiny a do slozek dle aktualniho dne.
    /// </summary>
    public abstract class Logger
    {

        /// <summary>
        /// Aktualni datum pro urceni slozky do ktere se maji zapisovat logy.
        /// </summary>
        private DateTime currentFolderDateTime;


        /// <summary>
        /// Formát názvu složky pro ukládání logů. Formát 'yyyy-MM-dd' odpovídá formátu rok-měsíc-den.
        /// </summary>
        private const string FOLDER_NAME_FORMAT = "yyyy-MM-dd";


        /// <summary>
        /// Cesta ke složce, kam se budou ukládat logy.
        /// </summary>
        protected readonly string RootPath;


        /// <summary>
        /// Vrací cestu k aktuální složce s logy.
        /// </summary>
        public string CurrentFolderPath
        {
            get
            {
                return Path.Combine(this.RootPath, this.currentFolderDateTime.ToString(FOLDER_NAME_FORMAT));
            }
        }

        /// <summary>
        /// Kontruktor abstraktni tridy.
        /// Nastavi cestu k slozce kam se maji logy ukladat.
        /// </summary>
        /// <param name="path">cesta ke slozce s logy.</param>
        protected Logger(string path)
        {
	        if (path == null) {
		        throw new ArgumentNullException("cesta k souboru je null");
	        }

	        if (!Directory.Exists(path)) {
		        throw new ArgumentException("Zadaná cesta neexistuje: " + path);
	        }

	        this.RootPath = path;
        }


        /// <summary>
        /// Určí zda existuje složka odpovídající zadanému datu a času.
        /// </summary>
        /// <param name="dateTime">datum a čas pro určení složky s logy.</param>
        /// <returns>true jestli složka existuje, jinak false.</returns>
        public bool FolderExists(DateTime dateTime)
        {
            return Directory.Exists(this.GetFolderPathByDate(dateTime));
        }


        /// <summary>
        /// Vrací cestu ke složce odpovídající zadanému datu. Složka nemusí existovat!
        /// </summary>
        /// <param name="dateTime">datum odpovídající složce.</param>
        /// <returns>cestu ke složce.</returns>
        public string GetFolderPathByDate(DateTime dateTime)
        {
            return Path.Combine(this.RootPath, dateTime.ToString(FOLDER_NAME_FORMAT));
        }


        /// <summary>
        /// Vytvoří novou složku v závislosti na čase. Formát složky je 'yyyy-MM-dd'.
        /// </summary>
        /// <param name="dateTime">datum pro název složky.</param>
        private void CreateFolder(DateTime dateTime)
        {
            Directory.CreateDirectory(Path.Combine(this.RootPath, dateTime.ToString(FOLDER_NAME_FORMAT)));
        }


        /// <summary>
        /// Nastaví cestu k jine slozce pokud se zmenil den.
        /// </summary>
        /// <param name="dateTime">datum pro název složky.</param>
        protected void SetFolderPathIfNewDayHasCome(DateTime dateTime)
        {
	        if (dateTime.Date >= this.currentFolderDateTime.Date) {
		        this.CreateFolder(dateTime);
		        this.currentFolderDateTime = dateTime;
	        }
        }
    }
}
