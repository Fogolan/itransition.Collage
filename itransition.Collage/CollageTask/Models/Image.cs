using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollageTask.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public string UserId { get; set; }

        public string Effects { get; set; }
    }
}