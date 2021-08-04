using System;
using AspNetCore.Identity.Mongo.Model;

namespace DevMeeting.Data.Entities.User
{
    public class UserRole : MongoRole<string>
    {
        public DateTime CreationDate { get; set; }
        
        public DateTime ModifiedDate { get; set; }
    }
}