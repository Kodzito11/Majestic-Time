using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchLibrary.Models;
namespace WatchLibrary.Repositories
{
	public class WatchRepository
	{
		private int _nextId = 1; // Holder styr på næste unikke ID
		private readonly List<Watch> _watches = new(); // Liste over ure

		public WatchRepository()
		{
			// Tilføjer nogle starture ved oprettelse af repository
			_watches.Add(new Watch { Id = _nextId++, Brand = "Rolex", Model = "Hulk", ReferenceNumber = "783289", Year = 1900, Accessories = "Med certifikat", Functions = "Special", Size = 40, Condition = 9 });
			_watches.Add(new Watch { Id = _nextId++, Brand = "Hublot", Model = "Big Bang", ReferenceNumber = "783289", Year = 2000, Accessories = "Med certifikat", Functions = "Special", Size = 40, Condition = 9 });
			_watches.Add(new Watch { Id = _nextId++, Brand = "Seiko", Model = "Prospex", ReferenceNumber = "783289", Year = 1900, Accessories = "Med certifikat", Functions = "Special", Size = 40, Condition = 9 });
		}

		public List<Watch> GetAll()
		{
			// Returnerer en kopi af listen for at beskytte interne data
			return new List<Watch>(_watches);
		}

		public Watch? GetById(int id)
		{
			// Finder et ur baseret på ID
			return _watches.Find(watch => watch.Id == id);
		}

		public Watch Add(Watch watch)
		{
			// Validerer ur før tilføjelse
			watch.Id = _nextId++; // Tildeler et unikt ID
			_watches.Add(watch);
			return watch;
		}

		public Watch? Remove(int id)
		{
			// Finder uret, der skal fjernes
			Watch? watch = GetById(id);
			if (watch == null)
			{
				return null; // Returnerer null, hvis uret ikke findes
			}
			_watches.Remove(watch);
			return watch;
		}

		public Watch? Update(int id, Watch watch)
		{
			Watch? existingWatch = GetById(id);
			if (existingWatch == null)
			{
				return null;
			}
			existingWatch.Brand = watch.Brand;
			existingWatch.Model = watch.Model;
			existingWatch.ReferenceNumber = watch.ReferenceNumber;
			existingWatch.Year = watch.Year;
			existingWatch.Accessories = watch.Accessories;
			existingWatch.Functions = watch.Functions;
			existingWatch.Size = watch.Size;
			existingWatch.Condition = watch.Condition;
			return existingWatch;
		}
	}
}