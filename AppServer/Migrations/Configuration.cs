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
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ARApplicationServer.DAL context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            context.Targets.AddOrUpdate(new Models.Target
            {
                ID = 1,
                Name = "database",
                DatFilePath = "F:\\Dropbox\\UNBC\\Thesis\\visual studio 2013\\ARapplicationServer\\AppServer\\Targets\\database.dat",
                XmlFilePath = "F:\\Dropbox\\UNBC\\Thesis\\visual studio 2013\\ARapplicationServer\\AppServer\\Targets\\database.xml"
            });
            context.Servers.AddOrUpdate(new Models.Server { ID = 1, Name = "Servertest" });
            context.Users.AddOrUpdate(new Models.User { ID = 1, Name = "Rahim", Password = "123" });
            context.Tags.AddOrUpdate(new Models.Tag { ID = 1, TargetID = 1, tag = "chip" });
            context.Tags.AddOrUpdate(new Models.Tag { ID = 2, TargetID = 1, tag = "lab" });
            context.Tags.AddOrUpdate(new Models.Tag { ID = 3, TargetID = 1, tag = "test" });
        }
    }
}
