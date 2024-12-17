using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Automate.Models
{
	public class User
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("Username")]
		public string Username { get; set; } = string.Empty;

		[BsonElement("Password")]
		public string PasswordHash { get; set; } = string.Empty;

		[BsonElement("IsAdmin")]
		public bool IsAdmin { get; set; } = false;

		[BsonElement("IsDeleted")]
		public bool IsDeleted { get; set; } = false;
	}
}