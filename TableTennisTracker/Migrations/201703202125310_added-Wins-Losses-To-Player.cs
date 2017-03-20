namespace TableTennisTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedWinsLossesToPlayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "Wins", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "Losses", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "Losses");
            DropColumn("dbo.Players", "Wins");
        }
    }
}
