namespace ARApplicationServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserAdded1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
    "dbo.Users",
    c => new
    {
        Id = c.Int(nullable: false, identity: true),
        Name = c.String(),
        Passwrord = c.String()
    })
    .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
