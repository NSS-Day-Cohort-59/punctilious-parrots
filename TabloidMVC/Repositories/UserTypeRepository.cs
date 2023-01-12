using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class UserTypeRepository : BaseRepository, IUserTypeRepository
    {
        public UserTypeRepository(IConfiguration config) : base(config) { }
        public List<UserType> GetUserTypes()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT
                    UserType.Id 'ID',
                    UserType.Name 'User Type'
                    FROM UserType
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<UserType> userTypes = new List<UserType>();

                        while (reader.Read())
                        {
                            userTypes.Add(NewUserTypeFromReader(reader));
                        }
                        return userTypes;
                    }
                }
            }
        }
        private static UserType NewUserTypeFromReader(SqlDataReader reader)
        {

            UserType newType = new UserType
            {
                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                Name = reader.GetString(reader.GetOrdinal("User Type"))
            };

            return newType;
        }
    }
}
