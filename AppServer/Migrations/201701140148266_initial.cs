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
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                })
                .PrimaryKey(t => t.Id);
            CreateTable(
    "dbo.Targets",
    c => new
    {
        Id = c.Int(nullable: false, identity: true),
        Name = c.String(),
    })
    .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.Servers");
            DropTable("dbo.Targets");
        }
    }
}
