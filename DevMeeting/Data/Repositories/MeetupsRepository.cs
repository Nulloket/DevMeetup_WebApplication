using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevMeeting.Data.Entities;
using DevMeeting.Models.SettingModels;
using DevMeeting.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DevMeeting.Data.Repositories
{
    public interface IMeetupsRepository
    {
        Task<Meetup> CreateMeetup(Meetup model);
        Task<Meetup> GetMeetupById(string id);
        Task<List<Meetup>> GetMeetups(int skip = 0, int limit = int.MaxValue);
        Task<List<Meetup>> GetMeetupsByPage(int pageIndex = 0, int perPage = int.MaxValue);
        Task<Meetup> GetMeetupsByOwnerId(string id);
        Task<bool> RemoveMeetupById(string id);
        Task<bool> ReplaceMeetupById(string id, Meetup model);
    }

    public class MeetupsRepository : IMeetupsRepository
    {
        private readonly ILogger _logger;
        private readonly IMongoDb _db;
        private readonly string _collectionName;
        public MeetupsRepository(IDatabaseSettings databaseSettings, ILogger<MeetupsRepository> logger)
        {
            _logger = logger;
            _db = new MongoDb(databaseSettings.ConnectionString, databaseSettings.DatabaseName);
            _collectionName = databaseSettings.MeetupsCollection;
        }

        public async Task<Meetup> CreateMeetup(Meetup model)
        {
            model.CreationDate = DateTime.Now;
            var response = await _db.InsertDocumentAsync<Meetup>(model, _collectionName);
            return response;
        }

        public async Task<Meetup> GetMeetupById(string id)
        {
            var filter = Builders<Meetup>.Filter.Where(meetup => meetup.MeetupId == id);
            var sort = Builders<Meetup>.Sort.Descending(meetup => meetup.CreationDate);
            var response = await _db.GetDocumentAsync(filter, _collectionName, sort);
            return response;
        }

        public async Task<List<Meetup>> GetMeetups(int skip = 0, int limit = int.MaxValue)
        {
            var sort = Builders<Meetup>.Sort.Descending(meetup => meetup.CreationDate);
            var filter = Builders<Meetup>.Filter.Empty;
            var response = await _db.GetDocumentsAsync<Meetup>(_collectionName, filter, skip, limit, sort);
            return response;
        }

        public async Task<List<Meetup>> GetMeetupsByPage(int pageIndex = 0, int perPage = int.MaxValue)
        {
            // TODO: Delete after completion
            // Truth Table
            // pI = 0 - pP = 20 | skip = 0 - limit = 20 => 0-20
            // pI = 1 - pP = 20 | skip = 21 - limit = 20 => 21-41
            // pI = 2 - pP = 20 | skip = 42 - limit = 20 => 42-62 
            // pI = 3 - pP = 20 | skip = 2 - limit = 20 => 63-83
            var skip = pageIndex == 0 ? pageIndex : (pageIndex * perPage) + pageIndex;
            var sort = Builders<Meetup>.Sort.Descending(meetup => meetup.CreationDate);
            var filter = Builders<Meetup>.Filter.Empty;
            var response = await _db.GetDocumentsAsync<Meetup>(_collectionName,filter, skip, perPage, sort);
            return response;
        }
        
        public async Task<Meetup> GetMeetupsByOwnerId(string id)
        {
            var filter = Builders<Meetup>.Filter.Where(meetup => meetup.UserId == id);
            var sort = Builders<Meetup>.Sort.Descending(meetup => meetup.CreationDate);
            var response = await _db.GetDocumentAsync(filter, _collectionName, sort);
            return response;
        }

        public async Task<bool> RemoveMeetupById(string id)
        {
            var filter = Builders<Meetup>.Filter.Where(meetup => meetup.MeetupId == id);
            var response = await _db.DeleteDocumentAsync(filter, _collectionName);
            return response;
        }
        
        public async Task<bool> ReplaceMeetupById(string id, Meetup model)
        {
            var filter = Builders<Meetup>.Filter.Where(meetup => meetup.MeetupId == id);
            var response = await _db.ReplaceDocument(filter, model, _collectionName);
            return response;
        }
    }
}