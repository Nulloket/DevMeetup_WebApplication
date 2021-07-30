using AutoMapper;
using DevMeeting.Models.Meetup;

namespace DevMeeting.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Data.Entities.Meetup, Meetup.MeetupModel>()
                .ReverseMap();
            CreateMap<MeetupCreate, Data.Entities.Meetup>();
        }
    }
}