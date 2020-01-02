﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Shuai.IdentityServer.V1._0.Test
{
    public class BlogService
    {
        private BloggingContext _context;

        public BlogService(BloggingContext context)
        {
            _context = context;
        }

        public Blog AddBlog(string name, string url)
        {
            var blog = _context.Blogs.Add(new Blog { Name = name, Url = url });
            _context.SaveChanges();

            return blog;
        }

        public List<Blog> GetAllBlogs()
        {
            var query = from b in _context.Blogs
                        orderby b.Name
                        select b;

            return query.ToList();
        }

        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            var query = from b in _context.Blogs
                        orderby b.Name
                        select b;

            return await query.ToListAsync();
        }

        public async Task<Blog> GetBlog()
        {
            return await _context.Blogs.FirstOrDefaultAsync(b => b.Name == "AAA");
        }
    }
}
