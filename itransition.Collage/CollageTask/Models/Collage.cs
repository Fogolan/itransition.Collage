using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollageTask.Models
{
    public class Collage
    {
        public int Id { get; set; }

        public string CollageName { get; set; }

        public string JSON { get; set; }

        public string Path { get; set; }

        public List<Image> Photos { get; set; }

        public string UserId { get; set; }

    }
}