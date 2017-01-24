namespace ARApplicationServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HubIDadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Servers", "HubID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Servers", "HubID");
        }
    }
}
