using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        public TagRepository(IConfiguration config) : base(config) { }

        public List<Tag> GetAllTags()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name
                                        FROM Tag";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Tag> tags = new List<Tag>();

                        while (reader.Read())
                        {
                            tags.Add(NewTagFromReader(reader));
                        }

                        return tags;

                    }
                }
            }
        }

        public Tag GetTagById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name
                                        FROM Tag
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return NewTagFromReader(reader);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public void AddTag(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Tag (Name)
                                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", tag.Name);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTag(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"UPDATE Tag
                                        SET 
                                            [Name] = @name
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", tag.Name);
                    cmd.Parameters.AddWithValue("@id", tag.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteTag(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM Tag
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"DELETE FROM PostTag
                                        WHERE TagId = @id";

                    cmd.ExecuteNonQuery();
                }
            }
        }



        private Tag NewTagFromReader(SqlDataReader reader)
        {
            return new Tag
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Name = reader.GetString(reader.GetOrdinal("Name"))
            };
        }
    }
}