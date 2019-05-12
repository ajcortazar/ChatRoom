using Dapper;
using System.Data.SqlServerCe;
using Challenge.Chatroom.Db.Model;

namespace Challenge.Chatroom.Db.Context
{
    public interface IDataAccess
    {
        User GetUserByIdentification(string Identification);        
    }
}
