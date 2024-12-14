using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace Automate.Models
{
	public class ClimateSystem
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("Type")]
		public SystemTypes Type { get; set; }

		[BsonElement("IsActivated")]
		public bool IsActivated { get; set; } = false;

		public enum SystemTypes
		{
			Windows,
			Heat,
			Fan,
			Watering,
			Light
		}

		public ClimateSystem(SystemTypes type)
		{
			Id = ObjectId.GenerateNewId();
			Type = type;
		}
	}
}
