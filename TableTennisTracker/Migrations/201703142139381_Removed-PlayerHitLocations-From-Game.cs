namespace TableTennisTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedPlayerHitLocationsFromGame : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HitLocations", "Game_Id1", "dbo.Games");
            DropIndex("dbo.HitLocations", new[] { "Game_Id1" });
            DropColumn("dbo.HitLocations", "Game_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HitLocations", "Game_Id1", c => c.Int());
            CreateIndex("dbo.HitLocations", "Game_Id1");
            AddForeignKey("dbo.HitLocations", "Game_Id1", "dbo.Games", "Id");
        }
    }
}
