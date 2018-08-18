using Built.Micro.ImageCloud.Mongo;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Built.Micro.ImageCloud
{
    /*
     If you want to create a repository for already defined non-entity model

	public class UserRepository : Repository<Entity<User>>
	{
		public UserRepository(string connectionString) : base(connectionString) {}

		//custom method
		public User FindbyUsername(string username)
		{
			return First(i => i.Content.Username == username);
		}
	}

        Usage
Each method has multiple overloads, read method summary for additional parameters

	UserRepository repo = new UserRepository("mongodb://localhost/sample")

	//Get
	User user = repo.Get("58a18d16bc1e253bb80a67c9");

	//Insert
	User item = new User(){
		Username = "username",
		Password = "password"
	};
	repo.Insert(item);

	//Update
	//single property
	repo.Update(item, i => i.Username, "newUsername");

	//multiple property
	//Updater has many methods like Inc, Push, CurrentDate, etc.
	var update1 = Updater.Set(i => i.Username, "oldUsername");
	var update2 = Updater.Set(i => i.Password, "newPassword");
	repo.Update(item, update1, update2);

	//all entity
	item.Username = "someUsername";
	repo.Replace(item);

	//Delete
	repo.Delete(item);

	//Queries - all queries has filter, order and paging features
	var first = repo.First();
	var last = repo.Last();
	var search = repo.Find(i => i.Username == "username");
	var allItems = repo.FindAll();

	//Utils
	var count = repo.Count();
	var any = repo.Any(i => i.Username.Contains("user"));
         */

    public class MaterialRepository : Repository<Material>
    {
        public MaterialRepository(IConfiguration config) : base(config)
        {
        }

        //custom method
        //public Material FindbyUsername(string username)
        //{
        //    return First(i => i.Username == username);
        //}
    }
}