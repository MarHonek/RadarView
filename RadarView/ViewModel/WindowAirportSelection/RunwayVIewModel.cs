using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.ViewModel.WindowAirportSelection
{
	/// <summary>
	/// ViewModel pro zobrazení vzletových drah ve výběru letiště.
	/// </summary>
	public class RunwayViewModel
	{
		/// <summary>
		/// Název dráhy.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Délka dráhy (metry).
		/// </summary>
		public float Length { get; set; }

		/// <summary>
		/// Kurz počátku dráhy.
		/// </summary>
		public float StartCourse { get; set; }

		/// <summary>
		/// Kuzr konce dráhy.
		/// </summary>
		public float EndCourse { get; set; }

		/// <summary>
		/// Textové výjádření kurzu.
		/// </summary>
		public string CourseText
		{
			get { return this.StartCourse + "/" + this.EndCourse; }
		}

		public RunwayViewModel(string name, float length, float startCourse, float endCourse)
		{
			this.Name = name;
			this.Length = length;
			this.StartCourse = startCourse;
			this.EndCourse = endCourse;
		}
	}
}
