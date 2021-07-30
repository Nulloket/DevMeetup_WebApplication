using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevMeeting.Data.Entities
{
    public class Meetup
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string MeetupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}