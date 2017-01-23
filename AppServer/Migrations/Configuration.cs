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
            context.Targets.AddOrUpdate(new Models.Target
            {
                ID = 1,
                Name = "database",
                DatFilePath = "F:\\Dropbox\\UNBC\\Thesis\\visual studio 2013\\ARapplicationServer\\AppServer\\Targets\\database.dat",
                XmlFilePath = "F:\\Dropbox\\UNBC\\Thesis\\visual studio 2013\\ARapplicationServer\\AppServer\\Targets\\database.xml"
            });
            context.ServerInfo.AddOrUpdate(new Models.Server
            {
                ID = 1,
                Name = "ServerA",
                Address = "http://localhost:7204",
                ChatFolder = "Targets/Chat",
                HubAddress = "http://localhost:50728/api",
                Identifier = "12365477",
                IncomingFolder = "Targets / Incoming",
                OutgoingFolder = "Targets/Outgoing",
                OutgoingTargetName = "tarmac",
                Registered = "false",
                TargetsFolder = "Targets"
            });
            context.Users.AddOrUpdate(new Models.User { ID = 1, Name = "Rahim", Password = "123" });
            context.Tags.AddOrUpdate(new Models.Tag { ID = 1, TargetID = 1, tag = "chip" });
            context.Tags.AddOrUpdate(new Models.Tag { ID = 2, TargetID = 1, tag = "lab" });
            context.Tags.AddOrUpdate(new Models.Tag { ID = 3, TargetID = 1, tag = "test" });
        }
    }
}
