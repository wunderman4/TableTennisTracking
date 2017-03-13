namespace TableTennisTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedIsSelectedtoPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "IsSelected", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "IsSelected");
        }
    }
}
