using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel;

namespace Automate.Models
{
	public class ClimateSystem : INotifyPropertyChanged
	{
		[BsonId]
		public ObjectId Id { get; set; }

		[BsonElement("Type")]
		public SystemTypes Type { get; set; }

		private bool _isActivated;
		[BsonElement("IsActivated")]
		public bool IsActivated
		{
			get => _isActivated;
			set
			{
				if (_isActivated != value)
				{
					_isActivated = value;
					OnPropertyChanged(nameof(IsActivated));
				}
			}
		}

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

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	}
}
