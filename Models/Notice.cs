using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace DotsApi.Models
{
    public class Notice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("user_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public string Name { get; set; }
        
        [BsonDateTimeOptions]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime TimeCreated { get; set; }

        [BsonDateTimeOptions]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime TimeCompleted { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool IsCompleted { get; set; }
    }
}
