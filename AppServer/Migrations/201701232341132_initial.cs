namespace ARApplicationServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Servers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Identifier = c.String(),
                        Name = c.String(),
                        Address = c.String(),
                        RootFolder = c.String(),
                        HubAddress = c.String(),
                        Registered = c.String(),
                        TargetsFolder = c.String(),
                        IncomingFolder = c.String(),
                        OutgoingFolder = c.String(),
                        OutgoingTargetName = c.String(),
                        ChatFolder = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TargetID = c.Int(nullable: false),
                        tag = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Targets", t => t.TargetID, cascadeDelete: true)
                .Index(t => t.TargetID);
            
            CreateTable(
                "dbo.Targets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        XmlFilePath = c.String(),
                        DatFilePath = c.String(),
                        ChatFilePath = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "TargetID", "dbo.Targets");
            DropIndex("dbo.Tags", new[] { "TargetID" });
            DropTable("dbo.Users");
            DropTable("dbo.Targets");
            DropTable("dbo.Tags");
            DropTable("dbo.Servers");
        }
    }
}
