namespace TableTennisTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedGlobalStats : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.GlobalStats");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.GlobalStats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlayerWithMostWins = c.String(),
                        PlayerWithMostWinsId = c.Int(nullable: false),
                        MostWins = c.Int(nullable: false),
                        PlayerWithMostGames = c.String(),
                        PlayerWithMostGamesId = c.Int(nullable: false),
                        MostGamesPlayed = c.Int(nullable: false),
                        PlayerWithBestWinRatio = c.String(),
                        PlayerWithBestWinRatioId = c.Int(nullable: false),
                        BestWinRatio = c.Single(nullable: false),
                        PlayerWithGreatestAvgPointSpreadWins = c.String(),
                        PlayerWithGreatestAvgPointSpreadWinsId = c.Int(nullable: false),
                        BestAvgPointSpreadWins = c.Single(nullable: false),
                        PlayerWithLeastAvgPointSpreadLosses = c.String(),
                        PlayerWithLeastAvgPointSpreadLossesId = c.Int(nullable: false),
                        LeastAvgPointSpreadLosses = c.Single(nullable: false),
                        GameWithLongestVolleyHits = c.Int(nullable: false),
                        Player1GameWithLongestVolleyHits = c.String(),
                        Player1GameWithLongestVolleyHitsId = c.Int(nullable: false),
                        Player2GameWithLongestVolleyHits = c.String(),
                        Player2GameWithLongestVolleyHitsId = c.Int(nullable: false),
                        LongestVolleyHits = c.Int(nullable: false),
                        GameWithLongestVolleyTime = c.Int(nullable: false),
                        Player1GameWithLongestVolleyTime = c.String(),
                        Player1GameWithLongestVolleyTimeId = c.Int(nullable: false),
                        Player2GameWithLongestVolleyTime = c.String(),
                        Player2GameWithLongestVolleyTimeId = c.Int(nullable: false),
                        LongestVolleyTime = c.Single(nullable: false),
                        LastUpdated = c.DateTime(precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
