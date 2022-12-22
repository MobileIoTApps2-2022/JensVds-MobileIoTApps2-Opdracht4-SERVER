using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace server.Models;

public class Log
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? userId { get; set; }

    public string? action { get; set; }

    public string? timestamp { get; set; }
}