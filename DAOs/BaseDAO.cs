using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Spotify.Contracts.DAO;

namespace Spotify.DAOs;

public class BaseDAO : IDAO
{
    public MongoClient connection = new MongoClient("mongodb+srv://a_nice_username:Fp3dQMbOtHCiwZbl@spotifinedb.8699v.mongodb.net/");
}
