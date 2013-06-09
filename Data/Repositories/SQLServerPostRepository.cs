﻿using Application.Posts.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using Application.Posts.Entities;

namespace Data.Repositories
{
    public class SQLServerPostRepository : IPostRepository
    {
        private static readonly string tableName = "Posts";
        private readonly string connection = ConfigurationManager.ConnectionStrings["testBlog"].ConnectionString;
        private readonly string sqlCreateTable = "CREATE TABLE [dbo].[" + tableName  + "]( "
                    + "[Id] [uniqueidentifier] NOT NULL, "
                    + "[CreatedDateTime] [date] NOT NULL, "
                    + "[ModifiedDateTime] [date] NOT NULL, "
                    + "[Title] [nvarchar](max) NOT NULL, "
                    + "[Body] [nvarchar](max) NOT NULL, "
                    + "[Tags] [nvarchar](max) NOT NULL, "
                    + "[Author] [nvarchar](max) NOT NULL "
                    + "PRIMARY KEY (ID) "
                    + ")";
        private readonly string sqlCheckTableExists = "IF NOT EXISTS "
                    + "(SELECT * "
                    + "FROM sys.objects "
                    + "WHERE object_id = OBJECT_ID(N'[dbo].[" + tableName + "]') "
                    + "AND type IN (N'U'))";

        public SQLServerPostRepository()
        {
            SendCommand( sqlCheckTableExists + " BEGIN " + sqlCreateTable + " END" );
        }

        public void CreatePost( Post post )
        {
            var sql = string.Format( "INSERT INTO dbo.{0} (Id, CreatedDateTime, ModifiedDateTime, Title, Body, Tags, Author) " +
                "VALUES ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')", tableName, Guid.NewGuid(), post.date, post.date, post.title, post.body, post.tags[0], post.author );
            SendCommand( sql );
        }

        public IEnumerable<Post> GetAllPosts( string author )
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAllPosts()
        {

            var posts = new List<Post>();
            using( SqlConnection conn = new SqlConnection( connection ) )
            {
                try
                {
                    conn.Open();
                    SqlCommand sqlCommand = new SqlCommand( "dbo.GetAllPosts", conn );
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    posts = (List<Post>)MapToPost( sqlCommand.ExecuteReader() );
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
            return posts;
        }

        private IEnumerable<Post> MapToPost( SqlDataReader reader )
        {
            var posts = new List<Post>();
            while( reader.Read() )
            {
                var post = new Post
                {
                    title = reader["Title"].ToString(),
                    body = reader["Body"].ToString(),
                    author = reader["Author"].ToString(),
                    date = (DateTime)reader["CreatedDateTime"],
                    tags = new string[] { reader["Tags"].ToString() }
                };
                posts.Add( post );
            }
            return posts;
        }

        public void DeletePost( string author, DateTime date, string title )
        {
            throw new NotImplementedException();
        }

        private int SendCommand( string command )
        {
            var rowsAffected = 0;
            using( SqlConnection db = new SqlConnection( connection ) )
            {
                try
                {
                    db.Open();
                    SqlCommand myCommand = new SqlCommand( command, db );
                    rowsAffected = myCommand.ExecuteNonQuery();
                    db.Close();
                }
                catch( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
            return rowsAffected;
        }
    }
}