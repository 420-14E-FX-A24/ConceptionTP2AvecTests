using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace Automate.Models
{
    public class Task
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("Type")]
        public TypeEnum Type { get; set; }

        [BsonElement("Comment")]
        public string Comment { get; set; }

        [BsonElement("Date")]
        public DateTime? Date { get; set; }

        [BsonElement("IsDeleted")]
        public bool IsDeleted { get; set; } = false;

        public enum TypeEnum
        {
            Semis,
            Rempotage,
            Entretien,
            Arrosage,
            Récolte,
            Commandes,
            Événements
        }

		public Task(TypeEnum type = TypeEnum.Semis, DateTime? date = null, string comment = "")
		{
			Id = ObjectId.GenerateNewId();
			Type = type;
            Date = date ?? DateTime.Now.Date;
            Comment = comment;
		}
	}
}
