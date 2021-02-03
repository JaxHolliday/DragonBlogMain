using DragonBlog.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonBlog.Services
{
    public interface ISlugService
    {
        public string URLFriendly(string title);

        public bool IsUnique(ApplicationDbContext dbContext, string slug);
    }
}
