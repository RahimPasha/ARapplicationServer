namespace ARApplicationServer.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ARApplicationServer.DAL>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ARApplicationServer.DAL context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            context.Targets.AddOrUpdate(new Models.Target { ID = 1, Name = "targettest" });
            context.Servers.AddOrUpdate(new Models.Server { ID = 1, Name = "Servertest" });
            //context.Servers.AddOrUpdate(new Models.Tag { ID = 1,  });
        }
    }
}
