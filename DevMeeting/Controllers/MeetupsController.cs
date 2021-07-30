using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DevMeeting.Data.Entities;
using DevMeeting.Data.Repositories;
using DevMeeting.Models.Meetup;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace DevMeeting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MeetupsController : ControllerBase
    {
        private readonly IMeetupsRepository _meetupsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MeetupsController> _logger;

        public MeetupsController(IMeetupsRepository meetupsRepository,
            IMapper mapper, ILogger<MeetupsController>  logger)
        {
            _meetupsRepository = meetupsRepository;
            _mapper = mapper;
            _logger = logger;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<MeetupModel>>> Get()
        {
            var meetups = await _meetupsRepository.GetMeetups();
            var mappedMeetups = _mapper.Map<List<MeetupModel>>(meetups);
            return mappedMeetups;
        }
        
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<MeetupModel>> GetMeetup(string  id)
        {
            var meetup = await _meetupsRepository.GetMeetupById(id);
            if (meetup is null)
                return NotFound();
            var mappedMeetup = _mapper.Map<MeetupModel>(meetup);
            return mappedMeetup;
        }
        
        [HttpPost]
        public async Task<ActionResult<Meetup>> Create(MeetupCreate model)
        {
            await WriteOutIdentityInformation();
            if (!ModelState.IsValid)
                return BadRequest();
            var mappedMeetup = _mapper.Map<Meetup>(model);
            var response = await _meetupsRepository.CreateMeetup(mappedMeetup);
            if (response is null)
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong, Please try again");
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<MeetupModel>> Edit(MeetupModel model)
        {
            var mappedMeetup = _mapper.Map<Meetup>(model);
            var response = await _meetupsRepository.ReplaceMeetupById(model.MeetupId, mappedMeetup);
            if (response)
                return model;
            return NotFound();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<MeetupModel>> Delete(string id)
        {
            var response = await _meetupsRepository.RemoveMeetupById(id);
            if (response)
                return NoContent();
            return NotFound();
        }

        public async Task WriteOutIdentityInformation()
        {
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            
            _logger.LogInformation($"Identity Token:  {identityToken}");

            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
            
        }
    }
}