#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevMeeting.Data.Entities;
using DevMeeting.Data.Entities.User;
using DevMeeting.Models.SettingModels;
using DevMeeting.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DevMeeting.Data.Repositories
{
    public interface IUserRepository
    {
        public Task<User> CreateUser(User model);
        public Task<User> GetUserById(string id);
        public Task<User> GetUserByUsername(string username);
        public Task<bool> UserExistsByEmail(string email);
        public Task<bool> UserExistsById(string id);
        public Task<bool> UserExists(User model);
        public Task<List<User>> GetUsers(int skip = 0, int limit = int.MaxValue);
        public Task<List<User>> GetUsersByPage(int pageIndex = 0, int perPage = int.MaxValue);
        public Task<List<User>> GetUserByRoleId(string role);
        public Task<List<User>> GetUsersByRoles(List<string> roles);
        public Task<User> GetUserByEmail(string  email);
        Task<bool> RemoveUserById(string id);
        Task<bool> ReplaceUser(User model);
    }
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly IMongoDb _db;
        private readonly string _collectionName;
        
        public UserRepository(IDatabaseSettings databaseSettings, ILogger<UserRepository> logger)
        {
            _logger = logger;
            _db = new MongoDb(databaseSettings.ConnectionString, databaseSettings.DatabaseName);
            _collectionName = databaseSettings.UsersCollection;
            
        }

        public async Task<User> CreateUser(User model)
        {
            model.CreationDate = DateTime.UtcNow;
            var response = await _db.InsertDocumentAsync(model, _collectionName);
            return response;
        }

        public async Task<User> GetUserById(string id)
        {
            var filter = Builders<User>.Filter.Where(user => user.Id == id);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var filter = Builders<User>.Filter.Where(user => user.UserName == username);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response;        }

        public async Task<bool> UserExistsByEmail(string email)
        {
            var filter = Builders<User>.Filter.Where(user => user.Email == email);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response is not null;
        }

        public async Task<bool> UserExistsById(string id)
        {
            var filter = Builders<User>.Filter.Where(user => user.Id == id);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response is not null;
        }

        public async Task<bool> UserExists(User model)
        {
            var filter = Builders<User>.Filter.Where(user => user.Id == model.Id);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response is not null;
        }

        public async Task<List<User>> GetUsers(int skip = 0, int limit = Int32.MaxValue)
        {
            var sort = Builders<User>.Sort.Descending(user => user.CreationDate);
            var filter = Builders<User>.Filter.Empty;
            var response = await _db.GetDocumentsAsync(_collectionName, filter, skip, limit, sort);
            return response;
        }

        public async Task<List<User>> GetUsersByPage(int pageIndex = 0, int perPage = Int32.MaxValue)
        {
            var skip = pageIndex == 0 ? pageIndex : (pageIndex * perPage) + pageIndex;
            var sort = Builders<User>.Sort.Descending(user => user.CreationDate);
            var filter = Builders<User>.Filter.Empty;
            var response = await _db.GetDocumentsAsync(_collectionName,filter, skip, perPage, sort);
            return response;
        }

        public Task<List<User>> GetUserByRoleId(string roleId)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersByRoles(List<string> roleIds)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var filter = Builders<User>.Filter.Where(user => user.NormalizedEmail == email);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response;
        }

        public async Task<bool> RemoveUserById(string id)
        {
            var filter = Builders<User>.Filter.Where(user => user.Id == id);
            var response = await _db.DeleteDocumentAsync(filter, _collectionName);
            return response;
        }

        public async Task<bool> ReplaceUser(User model)
        {
            model.ModifiedDate = DateTime.UtcNow;
            var filter = Builders<User>.Filter.Where(user => user.Id == model.Id);
            var response = await _db.ReplaceDocument(filter, model, _collectionName);
            return response;
        }
    }
}