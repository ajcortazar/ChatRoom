using Dapper;
using System.Data.SqlServerCe;
using Challenge.Chatroom.Db.Model;
using Challenge.Chatroom.Models.Helper;

namespace Challenge.Chatroom.Db.Context
{
    public class DataAccess
    {
        private SqlCeConnection Connection { get; set; }

        public DataAccess(string ConnectionString)
        {
            Connection = new SqlCeConnection(ConnectionString);
        }

        public SqlCeConnection GetConnection()
        {
            if(Connection.State == System.Data.ConnectionState.Closed)
            {
                Connection.Open();
            }

            return Connection;
        }

        public User GetUserByIdentification(string Identification)
        {
            var query = "SELECT * FROM [User] WHERE Identification = '" + Identification + "'";
            var user = GetConnection().QueryFirstOrDefault<User>(query, new { Identification });
            return user;
        }

        public void RegisterUser(User user)
        {
            user.Password = Utilities.EncryptMd5(user.Password);
            var query = "INSERT INTO [User] (FullName, Identification, Password, Active)";
            query += string.Format("VALUES('{0}','{1}','{2}','{3}')", user.FullName, user.Identification, user.Password, true);

            GetConnection().Execute(query);
        }
    }
}
