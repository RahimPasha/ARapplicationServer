namespace ARApplicationServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TagAdded : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Servers", "Identifier", c => c.String());
            AddColumn("dbo.Servers", "Address", c => c.String());
            AddColumn("dbo.Targets", "XmlFilePath", c => c.String());
            AddColumn("dbo.Targets", "DatFilePath", c => c.String());
            AddColumn("dbo.Targets", "ChatFilePath", c => c.String());
            AlterColumn("dbo.Targets", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tags", "TargetID", "dbo.Targets");
            DropIndex("dbo.Tags", new[] { "TargetID" });
            AlterColumn("dbo.Targets", "Name", c => c.String());
            DropColumn("dbo.Targets", "ChatFilePath");
            DropColumn("dbo.Targets", "DatFilePath");
            DropColumn("dbo.Targets", "XmlFilePath");
            DropColumn("dbo.Servers", "Address");
            DropColumn("dbo.Servers", "Identifier");
            DropTable("dbo.Tags");
        }
    }
}
