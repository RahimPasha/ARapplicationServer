using System.Data.Entity;
using ARApplicationServer.Models;
namespace ARApplicationServer
{
    public class DAL:DbContext
    {
        public DAL(): base("cnnStr") { }

        public DbSet<Models.Target> Targets { get; set; }
        public DbSet<Models.Server> Servers { get; set; }
    }
}