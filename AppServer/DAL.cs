using System.Data.Entity;
using ARApplicationServer.Models;
namespace ARApplicationServer
{
    public class DAL:DbContext
    {
        public DAL(): base("cnnStr") { }

        public DbSet<Target> Targets { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}