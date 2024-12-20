﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_event.Models.User;

public class UserDto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Mail { get; set; } = null!;
}