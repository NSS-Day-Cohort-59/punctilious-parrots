using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration config) : base(config) { }

        public UserProfile GetByEmail(string email)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT u.id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                              u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                              ut.[Name] AS UserTypeName
                         FROM UserProfile u
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE email = @email";
                    cmd.Parameters.AddWithValue("@email", email);

                    UserProfile userProfile = null;
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            ImageLocation = DbUtils.GetNullableString(reader, "ImageLocation"),
                            UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                            UserType = new UserType()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                            },
                        };
                    }

                    reader.Close();

                    return userProfile;
                }
            }
        }

        public List<UserProfile> GetUsers()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT
                    UserProfile.Id 'ID',
                    FirstName,
                    LastName,
                    DisplayName,
                    Email,
                    ImageLocation,
                    CreateDateTime,
                    UserTypeId,
                    UserType.Name 'User Type'
                    FROM UserProfile
                    LEFT JOIN UserType
                    ON UserProfile.UserTypeId = UserType.Id
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<UserProfile> users = new List<UserProfile>();

                        while (reader.Read())
                        {
                            users.Add(NewUserFromReader(reader));
                        }
                        return users;
                    }
                }
            }
        }

        public void Add(UserProfile newUser)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO UserProfile (
                            FirstName,
                            LastName,
                            DisplayName,
                            Email,
                            ImageLocation,
                            CreateDateTime,
                            UserTypeId
                            )
                        OUTPUT INSERTED.ID
                        VALUES (
                            @FirstName,
                            @LastName,
                            @DisplayName,
                            @Email,
                            @ImageLocation,
                            @CreateDateTime,
                            @UserTypeId
                            )";
                    cmd.Parameters.AddWithValue("@FirstName", newUser.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", newUser.LastName);
                    cmd.Parameters.AddWithValue("@DisplayName", newUser.DisplayName);
                    cmd.Parameters.AddWithValue("@Email", newUser.Email);
                    cmd.Parameters.AddWithValue("@ImageLocation", DbUtils.ValueOrDBNull(newUser.ImageLocation));
                    cmd.Parameters.AddWithValue("@CreateDateTime", newUser.CreateDateTime);
                    cmd.Parameters.AddWithValue("@UserTypeId", newUser.UserTypeId);

                    newUser.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public UserProfile GetUserById(int userId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    SELECT
                    UserProfile.Id 'ID',
                    FirstName,
                    LastName,
                    DisplayName,
                    Email,
                    ImageLocation,
                    CreateDateTime,
                    UserTypeId,
                    UserType.Name 'User Type'
                    FROM UserProfile
                    LEFT JOIN UserType
                    ON UserProfile.UserTypeId = UserType.Id
                    WHERE UserProfile.Id = @UserId
                    ";
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    
                    
                    var reader = cmd.ExecuteReader();
                    UserProfile userProfile = null;

                    if (reader.Read())
                    {
                        userProfile = NewUserFromReader(reader);
                    }
                    reader.Close();
                    return userProfile;
                }
            }
        }    

        private UserProfile NewUserFromReader(SqlDataReader reader)
        {
            
            UserProfile newUser = new UserProfile
            {
                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                ImageLocation = DbUtils.GetNullableString(reader, "ImageLocation"),
                UserType = new UserType
                {
                    Name = reader.GetString(reader.GetOrdinal("User Type"))
                }
            };
            if (newUser.ImageLocation == null)
            {
                newUser.ImageLocation = "https://villagesonmacarthur.com/wp-content/uploads/2020/12/Blank-Avatar.png";
            }
            return newUser;
        }
    }
}