using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Spotify.Models.DTOs;

public class GenreDTO
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("name")]
    public string name { get; set; }
}
