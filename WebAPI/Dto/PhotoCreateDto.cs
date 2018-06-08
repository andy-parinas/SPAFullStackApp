using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Dto
{
    public class PhotoCreateDto
    {
        public string UrlString { get; set; }

        public IFormFile File { get; set; }

        public string  Description { get; set; }

        public DateTime DateAdded { get; set; }

        public string PublicId { get; set; }

        public PhotoCreateDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}
