namespace TableTennisTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedvolleytohitLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HitLocations", "Volley", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HitLocations", "Volley");
        }
    }
}
