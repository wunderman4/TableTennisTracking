namespace TableTennisTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GamePlayers",
                c => new
                    {
                        GameId = c.Int(nullable: false),
                        PlayerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GameId, t.PlayerId })
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.PlayerId, cascadeDelete: true)
                .Index(t => t.GameId)
                .Index(t => t.PlayerId);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Player1Score = c.Int(nullable: false),
                        Player2Score = c.Int(nullable: false),
                        MaxVelocity = c.Single(nullable: false),
                        LongestVolleyTime = c.Single(nullable: false),
                        LongestVolleyHits = c.Int(nullable: false),
                        Player1_Id = c.Int(),
                        Player2_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Player1_Id)
                .ForeignKey("dbo.Players", t => t.Player2_Id)
                .Index(t => t.Player1_Id)
                .Index(t => t.Player2_Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        PlayerName = c.String(),
                        Age = c.Int(nullable: false),
                        HeightFt = c.Int(nullable: false),
                        HeightInch = c.Int(nullable: false),
                        Nationality = c.String(),
                        HandPreference = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HitLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                        Z = c.Single(nullable: false),
                        Game_Id = c.Int(),
                        Game_Id1 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.Game_Id)
                .ForeignKey("dbo.Games", t => t.Game_Id1)
                .Index(t => t.Game_Id)
                .Index(t => t.Game_Id1);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HitLocations", "Game_Id1", "dbo.Games");
            DropForeignKey("dbo.Games", "Player2_Id", "dbo.Players");
            DropForeignKey("dbo.HitLocations", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.Games", "Player1_Id", "dbo.Players");
            DropForeignKey("dbo.GamePlayers", "PlayerId", "dbo.Players");
            DropForeignKey("dbo.GamePlayers", "GameId", "dbo.Games");
            DropIndex("dbo.HitLocations", new[] { "Game_Id1" });
            DropIndex("dbo.HitLocations", new[] { "Game_Id" });
            DropIndex("dbo.Games", new[] { "Player2_Id" });
            DropIndex("dbo.Games", new[] { "Player1_Id" });
            DropIndex("dbo.GamePlayers", new[] { "PlayerId" });
            DropIndex("dbo.GamePlayers", new[] { "GameId" });
            DropTable("dbo.HitLocations");
            DropTable("dbo.Players");
            DropTable("dbo.Games");
            DropTable("dbo.GamePlayers");
        }
    }
}
