using server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace server.Services;

public class LogService
{
    private readonly IMongoCollection<Log> _LogsCollection;

    public LogService(
        IOptions<LogDatabaseSettings> LogDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            LogDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            LogDatabaseSettings.Value.DatabaseName);

        _LogsCollection = mongoDatabase.GetCollection<Log>(
            LogDatabaseSettings.Value.LogsCollectionName);
    }

    public async Task<List<Log>> GetAsync() =>
        await _LogsCollection.Find(_ => true).ToListAsync();

    public async Task<Log?> GetAsync(string Id) =>
        await _LogsCollection.Find(x => x.Id == Id).FirstOrDefaultAsync();

    public async Task CreateAsync(Log newLog) =>
        await _LogsCollection.InsertOneAsync(newLog);

    public async Task UpdateAsync(string Id, Log updatedLog) =>
        await _LogsCollection.ReplaceOneAsync(x => x.Id == Id, updatedLog);

    public async Task RemoveAsync(string Id) =>
        await _LogsCollection.DeleteOneAsync(x => x.Id == Id);
}