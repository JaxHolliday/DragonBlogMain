using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonBlog.Services
{
    public interface IImageService
    {
        //Task to return an Async, task type byte array
        Task<byte[]> EncodeFileAsync(IFormFile formFile);

        //to decode to screen
        string DecodeFile(byte[] imageData, string contentType);

        string RecordFileName(IFormFile formfile);
    }
}
