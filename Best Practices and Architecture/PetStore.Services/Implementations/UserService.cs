using System;

using PetStore.Data;
using PetStore.Data.Models;

namespace PetStore.Services.Implementations
{
	public class UserService : IUserService
	{
		private readonly PetStoreDbContext data;

		public UserService(PetStoreDbContext db)
		{
			this.data = db;
		}

		public bool Exists(int userId)
		{
			bool exists = true;

			var user = this.data
				.Users
				.Find(userId);

			if (user == null)
			{
				exists = false;
				throw new ArgumentException("User not found.");
			}

			return exists;
		}

		public void Register(string name, string email)
		{
			var user = new User
			{
				Name = name,
				Email = email
			};

			this.data.Users.Add(user);
			this.data.SaveChanges();
		}
	}
}
