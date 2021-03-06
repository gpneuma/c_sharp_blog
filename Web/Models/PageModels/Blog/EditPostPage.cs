﻿using Application.Posts.Entities;
using Application.Posts.Interactors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Web.Models.PageModels
{
    public class EditPostPage
    {
        public string PageHeader = "Edit Post";
        public EditPostPageForm Post;

        public EditPostPage( IPostInteractor postInteractor, string author, string title )
        {
            Post = new EditPostPageForm();
            var posts = postInteractor.GetAllPosts( author ).ToList();
            var post = posts.Find( p => p.Title == title.Replace( '_', ' ' ) );
            Post.Title = post.Title;
            Post.Body = post.Body;
            Post.Tags = ( ParseTags( post.Tags ) );
            Post.Author = post.Author;
            Post.Date = post.Date;
        }

        private string ParseTags( string[] tags )
        {
            var result = tags.First();
            if( tags.Count() > 1 )
                foreach( var tag in tags )
                {
                    result = string.Format( "{0}, {1}", result, tag );
                }
            return result;
        }
    }

    public class EditPostPageForm
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
    }
}