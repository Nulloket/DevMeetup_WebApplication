using System;
using System.Collections.Generic;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DevMeeting.Data.Entities.User
{
    public class User : MongoUser<string>
    {
        public bool IsActive { get; set; }
        
        public DateTime CreationDate { get; set; }
        
        public DateTime ModifiedDate { get; set; }
        
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
        [PersonalData]
        public DateTime BirthDate { get; set; }
        
    }
}