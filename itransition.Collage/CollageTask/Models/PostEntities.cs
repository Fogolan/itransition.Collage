using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CollageTask.Models
{
    public class PostEntities : DbContext
    {
        public DbSet<Image> Images { get; set; }
        public DbSet<Collage> Collages { get; set; }
    }
}