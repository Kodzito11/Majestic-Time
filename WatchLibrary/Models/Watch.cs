using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchLibrary.Models
{
	public class Watch
	{
		public int Id { get; set; }
		public string Brand { get; set; }
		public string Model { get; set; }
		public string ReferenceNumber { get; set; }
		public int Year { get; set; }
		public string? Accessories { get; set; } //Alle ure kommer ikke nøvendigvis med tilbehør
		public string? Functions { get; set; } //Alle ure har ikke nødvendigvis ekstra funktioner
		public int Size { get; set; }
		public int Condition { get; set; }




		public override string ToString()
		{
			return $"Id: {Id}, Brand: {Brand}, Model: {Model}, ReferenceNumber: {ReferenceNumber}, Year: {Year}, Accessories: {Accessories}, Functions: {Functions}, Size: {Size}, Condition: {Condition}";
		}
	}

}
