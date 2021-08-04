using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevMeeting.Data.Entities.User;
using DevMeeting.Models.SettingModels;
using DevMeeting.Utilities;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DevMeeting.Data.Repositories
{
    public interface IRoleRepository
    {
        public Task<UserRole> CreateRole(UserRole model);
        public Task<UserRole> GetRoleById(string id);
        public Task<UserRole> GetRoleByName(string name);
        public Task<List<UserRole>> GetRoles(int skip = 0, int limit = int.MaxValue);
        public Task<bool> ReplaceRole(UserRole model);
        public Task<bool> RemoveRole(UserRole model);

    }
    public class RoleRepository : IRoleRepository
    {
        private readonly ILogger<RoleRepository> _logger;
        private readonly IMongoDb _db;
        private readonly string _collectionName;
        
        public RoleRepository(ILogger<RoleRepository> logger,
            IDatabaseSettings databaseSettings)
        {
            _logger = logger;
            _db = new MongoDb(databaseSettings.ConnectionString, databaseSettings.DatabaseName);
            _collectionName = databaseSettings.RoleCollection;
        }

        public async Task<UserRole> CreateRole(UserRole model)
        {
            model.CreationDate = DateTime.UtcNow;
            var response = await _db.InsertDocumentAsync(model, _collectionName);
            return response;
        }

        public async Task<UserRole> GetRoleById(string id)
        {
            var filter = Builders<UserRole>.Filter.Where(role => role.Id == id);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response;
        }

        public async Task<UserRole> GetRoleByName(string name)
        {
            var filter = Builders<UserRole>.Filter.Where(role => role.NormalizedName == name);
            var response = await _db.GetDocumentAsync(filter, _collectionName);
            return response;
        }

        public async Task<List<UserRole>> GetRoles(int skip = 0, int limit = Int32.MaxValue)
        {
            var sort = Builders<UserRole>.Sort.Descending(role => role.CreationDate);
            var filter = Builders<UserRole>.Filter.Empty;
            var response = await _db.GetDocumentsAsync(_collectionName, filter, skip, limit, sort);
            return response;
        }

        public async Task<bool> ReplaceRole(UserRole model)
        {
            model.ModifiedDate = DateTime.UtcNow;
            var filter = Builders<UserRole>.Filter.Where(role => role.Id == model.Id);
            var response = await _db.ReplaceDocument(filter, model, _collectionName);
            return response;
        }

        public async Task<bool> RemoveRole(UserRole model)
        {
            var filter = Builders<UserRole>.Filter.Where(role => role.Id == model.Id);
            var response = await _db.DeleteDocumentAsync(filter, _collectionName);
            return response;
        }
    }
}