using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchLibrary
	{
		public class WatchRepository
		{
			private int _nextId = 1; // Holder styr på næste unikke ID
			private readonly List<User> _watches = new(); // Liste over brugere

			public WatchRepository()
			{
				// Tilføjer nogle startbrugere ved oprettelse af repository
				_watches.Add(new User { Id = _nextId++, Username = "Roman", Email = "roman@example.com", Password = "hashedpassword1" });
				_watches.Add(new User { Id = _nextId++, Username = "Kodo", Email = "kodo@example.com", Password = "hashedpassword2" });
				_watches.Add(new User { Id = _nextId++, Username = "Joutiar", Email = "Joutiar@example.com", Password = "hashedpassword3" });
			}

			public List<User> GetAll()
			{
				// Returnerer en kopi af listen for at beskytte interne data
				return new List<User>(_watches);
			}

			public User? GetById(int id)
			{
				// Finder en bruger baseret på ID
				return _watches.Find(user => user.Id == id);
			}

			public User Add(User user)
			{
				// Validerer bruger før tilføjelse
				user.Validate();
				user.Id = _nextId++; // Tildeler et unikt ID
				_watches.Add(user);
				return user;
			}

			public User? Remove(int id)
			{
				// Finder brugeren, der skal fjernes
				User? user = GetById(id);
				if (user == null)
				{
					return null; // Returnerer null, hvis brugeren ikke findes
				}
				_watches.Remove(user);
				return user;
			}

			public User? Update(int id, User user)
			{
				user.Validate();
				User? existingUser = GetById(id);
				if (existingUser == null)
				{
					return null;
				}
				existingUser.Username = user.Username;
				existingUser.Email = user.Email;
				existingUser.Password = user.Password;
				return existingUser;
			}
		}
	}