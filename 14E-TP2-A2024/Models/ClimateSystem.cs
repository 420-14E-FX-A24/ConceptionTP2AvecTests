using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace Automate.Models
{
	public class ClimateSystem
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("SystemType")]
		public SystemTypeEnum SystemType { get; set; }

		[BsonElement("UnitType")]
		public UnitTypeEnum UnitType { get; set; }

		[BsonElement("UnitValue")]
		public int UnitValue { get; set; } = 0;

		[BsonElement("IsActivated")]
		public bool IsActivated { get; set; } = false;

		public enum SystemTypeEnum
		{
			Windows,
			Heat,
			Fan,
			Watering,
			Light
		}

		public enum UnitTypeEnum
		{
			Humidity,
			Temperature,
			Luminosity
		}

		public ClimateSystem(SystemTypeEnum systemType)
		{
			Id = ObjectId.GenerateNewId();
			SystemType = systemType;

			switch (systemType) 
			{ 
				case SystemTypeEnum.Windows:
				case SystemTypeEnum.Heat:
				case SystemTypeEnum.Fan:
					UnitType = UnitTypeEnum.Temperature;
					break;
				case SystemTypeEnum.Watering:
					UnitType = UnitTypeEnum.Humidity;
					break;
				case SystemTypeEnum.Light:
					UnitType = UnitTypeEnum.Luminosity;
					break;
				default:
					break;
			}
		}
	}
}
