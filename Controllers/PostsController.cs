﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DragonBlog.Data;
using DragonBlog.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using DragonBlog.Utilities;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Authorization;
using PagedList;
using DragonBlog.Services;

namespace DragonBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ImageService _imageService;

        public PostsController(ApplicationDbContext context, ImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        
        //GET: Posts
        public async Task<IActionResult> Index()
        {
            var model = _context.Post.Include(p => p.Blog);            
            return View(await model.ToListAsync());
        }
 

        public async Task<IActionResult> BlogPosts(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog.FindAsync(id);
            if(blog == null)
            {
                return NotFound();
            }

            ViewData["BlogName"] = blog.Name;
            ViewData["BlogId"] = blog.Id;

            var blogPosts = await _context.Post.Where(p => p.BlogId == id).ToListAsync();
            return View(blogPosts);
        }
        
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var post = await _context.Post
                .Include(p => p.Blog)
                .Include(p => p.Comments).ThenInclude(p => p.Author)
                
                .FirstOrDefaultAsync(m => m.Id == id);
            
            
            var myImageHelper = new ImageHelper();
            if(post.Image != null)
            {
                ViewData["Image"] = myImageHelper.GetImage(post);
            }
            
            return View(post);

        }

        [Authorize(Roles = "Admin")]
        // GET: Posts/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Name");
                return View();
            }

            var blog = await _context.Blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            var newPost = new Post { BlogId = (int)id };
            return View(newPost);
           
            
           
        }

        [Authorize(Roles = "Admin")]
        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,IsPublished")] Post post, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                post.Created = DateTime.Now;

                //Image Service Created on: 2/5/21
                //post.FileName = _imageService.RecordFileName(image);
                //post.Image = await _imageService.EncodeFileAsync(image);

                if(image != null)
                {
                    post.FileName = image.FileName;

                    //Turn IMG into Binary
                    var ms = new MemoryStream();
                    image.CopyTo(ms);
                    post.Image = ms.ToArray();

                    ms.Close();
                    ms.Dispose();
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }  

            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Id", post.BlogId);
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Id", post.BlogId);
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,FileName,Image,Content,Created,IsPublished")] Post post, IFormFile image)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (image != null)
                    {
                        post.FileName = image.FileName;

                        //Turn IMG into Binary
                        var ms = new MemoryStream();
                        image.CopyTo(ms);
                        post.Image = ms.ToArray();

                        ms.Close();
                        ms.Dispose();
                    }

                    post.Created = DateTime.Now;
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blog, "Id", "Id", post.BlogId);
            return View(post);
        }

        [Authorize(Roles = "Admin")]
        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [Authorize(Roles = "Admin")]
        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Post.FindAsync(id);
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.Id == id);
        }
    }
}
