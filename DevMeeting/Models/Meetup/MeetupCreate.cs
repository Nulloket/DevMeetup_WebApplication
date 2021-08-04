using System;
using System.ComponentModel.DataAnnotations;

namespace DevMeeting.Models.Meetup
{
    public class MeetupCreate : BaseRequest
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(400)]
        public string Description { get; set; }
        [Required]
        [Url]
        public string ImageUrl { get; set; }
        [Timestamp]
        [Required]
        public DateTime DueDate { get; set; }
    }
}