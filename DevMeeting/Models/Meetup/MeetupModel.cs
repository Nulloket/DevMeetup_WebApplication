using System;

namespace DevMeeting.Models.Meetup
{
    public class MeetupModel
    {
        public string MeetupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string UserId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}