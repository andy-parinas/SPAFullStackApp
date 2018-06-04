using System;

namespace WebAPI.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public string  UrlString { get; set; }

        public string Description { get; set; }

        public DateTime  DateAdded { get; set; }

        public bool IsMain { get; set; }

        //Adding the 2 properies below defines the full relationship
        //It tells the Entity Framework to use the Cascade Delete
        public User User { get; set; }

        public int UserId { get; set; }


    }
}