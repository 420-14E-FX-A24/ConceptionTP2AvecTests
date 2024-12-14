using Automate.Models;
using MongoDB.Driver;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Automate.Utils
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Task> _tasks;
		private readonly IMongoCollection<ClimateSystem> _climateSystems;
		private readonly IMongoCollection<ClimateCondition> _climateConditions;

		public MongoDBService(string databaseName)
		{
			var client = new MongoClient("mongodb://localhost:27017");
			_database = client.GetDatabase(databaseName);
			_users = _database.GetCollection<User>("Users");
			_tasks = _database.GetCollection<Task>("Tasks");
			_climateSystems = _database.GetCollection<ClimateSystem>("ClimateSystems");

			AddFirstUser("Andre", false);
			AddFirstUser("Frederic", true);
		}

		private void AddFirstUser(string username, bool isAdmin)
		{
            var user = FindUserRoleFirstOrDefault(isAdmin);

            if (user is null) 
                RegisterUser(new User { Username = username, PasswordHash = "$2a$11$Rc0K8jktZrVizcxsNmEQU.c94VWEHjKxrmk0I09p5dkBteMSoJ2Bq", IsAdmin = isAdmin });
		}

		private User? FindUserRoleFirstOrDefault(bool isAdmin)
        {
            var filter = Builders<User>.Filter.Eq(user => user.IsAdmin, isAdmin);
            return _users.Find(filter).FirstOrDefault();
        }

		public virtual User? Authenticate(string? username, string? password)
		{
			var filter = Builders<User>.Filter.And(
				Builders<User>.Filter.Eq(u => u.Username, username),
				Builders<User>.Filter.Eq(u => u.IsDeleted, false)
			);

			var user = _users.Find(filter).FirstOrDefault();

			if (user is not null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
			{
				return user;
			}
			return null;
		}

		public ObservableCollection<Task> GetMonthTasks(DateTime date)
		{
			DateTime startOfMonth = new DateTime(date.Year, date.Month, 1);
			DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

			var filter = Builders<Task>.Filter.And(
				Builders<Task>.Filter.Gte(t => t.Date, startOfMonth),
				Builders<Task>.Filter.Lte(t => t.Date, endOfMonth),
				Builders<Task>.Filter.Eq(t => t.IsDeleted, false)
			);

			var tasks = _tasks.Find(filter).ToList();

			return new ObservableCollection<Task>(tasks);
		}

		public ObservableCollection<Task> GetTasks(DateTime date)
		{
			var filter = Builders<Task>.Filter.And(
				Builders<Task>.Filter.Eq(t => t.Date, date.Date),
				Builders<Task>.Filter.Eq(t => t.IsDeleted, false)
			);
			var tasks = _tasks.Find(filter).ToList();

			return new ObservableCollection<Task>(tasks);
		}

		public void RegisterUser(User user)
        {
            _users.InsertOne(user);
        }

        public void SaveTask(Task task)
        {
			var filter = Builders<Task>.Filter.Eq(t => t.Id, task.Id);

			_tasks.ReplaceOne(filter, task, new ReplaceOptions { IsUpsert = true });
        }

        public void RemoveTask(Task task)
        {
			var filter = Builders<Task>.Filter.Eq(t => t.Id, task.Id);
			var update = Builders<Task>.Update.Set(t => t.IsDeleted, true);

			_tasks.UpdateOne(filter, update);
		}

		public ObservableCollection<ClimateSystem> GetClimateSystems()
		{
			var filter = Builders<ClimateSystem>.Filter.Empty;
			var climateSystems = _climateSystems.Find(filter).ToList();

			return new ObservableCollection<ClimateSystem>(climateSystems);
		}

		public void SaveClimateSystem(ClimateSystem climateSystem)
		{
			var filter = Builders<ClimateSystem>.Filter.Eq(c => c.Id, climateSystem.Id);

			_climateSystems.ReplaceOne(filter, climateSystem, new ReplaceOptions { IsUpsert = true });
		}

		public ObservableCollection<ClimateCondition> GetClimateConditions()
		{
			var filter = Builders<ClimateCondition>.Filter.Empty;
			var climateConditions = _climateConditions.Find(filter).ToList();

			return new ObservableCollection<ClimateCondition>(climateConditions);
		}

		public void SaveClimateCondition(ClimateCondition climateCondition)
		{
			var filter = Builders<ClimateCondition>.Filter.Eq(c => c.Id, climateCondition.Id);

			_climateConditions.ReplaceOne(filter, climateCondition, new ReplaceOptions { IsUpsert = true });
		}

	}

}
