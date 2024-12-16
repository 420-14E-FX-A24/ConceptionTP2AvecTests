using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace Automate.Models
{
	public class ClimateCondition
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("Type")]
		public ConditionTypes Type { get; set; }

		[BsonElement("IsDay")]
		public bool IsDay { get; set; }

		[BsonElement("RealTimeValue")]
		public int? RealTimeValue { get; set; }

		[BsonElement("MinRangeValue")]
		public int MinRangeValue { get; set; }

		[BsonElement("MaxRangeValue")]
		public int MaxRangeValue { get; set; }

		public enum ConditionTypes
		{
			Humidity,
			Temperature,
			Luminosity
		}

		public ClimateCondition(ConditionTypes type, int minRangeValue, int maxRangeValue, bool isDay)
		{
			Id = ObjectId.GenerateNewId();
			Type = type;
			MinRangeValue = minRangeValue;
			MaxRangeValue = maxRangeValue;
			IsDay = isDay;
		}
	}
}
