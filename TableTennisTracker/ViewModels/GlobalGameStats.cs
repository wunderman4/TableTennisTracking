using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTennisTracker.Models;
using TableTennisTracker.ModelViews;
using TableTennisTracker.Respository;
using TableTennisTracker.Services;
using TableTennisTracker.ViewModels;

namespace TableTennisTracker.ModelViews
{
    public class GlobalGameStats
    {
        private GenericRespository _repo;

        TableTennisTrackerDb _db = new TableTennisTrackerDb();
                
        private GlobalGameStatsView globalStats;


        public string PlayerWithMostWins { get; set; }
        public int PlayerWithMostWinsId { get; set; }
        public string MostWins { get; set; }

        public int GameWithLongestVolleyHits { get; set; }
        public string Player1GameWithLongestVolleyHits { get; set; }
        public int Player1GameWithLongestVolleyHitsId { get; set; }
        public string Player2GameWithLongestVolleyHits { get; set; }
        public int Player2GameWithLongestVolleyHitsId { get; set; }
        public string LongestVolleyHits { get; set; }

        public int GameWtihLongestVolleyTime { get; set; }
        public string Player1GameWithLongestVolleyTime { get; set; }
        public int Player1GameWithLongestVolleyTimeId { get; set; }
        public string Player2GameWithLongestVolleyTime { get; set; }
        public int Player2GameWithLongestVolleyTimeId { get; set; }
        public string LongestVolleyTime { get; set; }

        public string PlayerWithMostGames { get; set; }
        public int PlayerWithMostGamesId { get; set; }
        public string MostGames { get; set; }

        public string PlayerWithBestWinRatio { get; set; }
        public int PlayerWithBestWinRatioId { get; set; }
        public string BestWinRatio { get; set; }

        public string PlayerWithGreatestAvgPointSpreadWins { get; set; }
        public int PlayerWithGreatestAvgPointSpreadWinsId { get; set; }
        public string BestAvgPointSpreadWins { get; set; }

        public string PlayerWithLeastAvgPointSpreadLosses { get; set; }
        public int PlayerWithLeastAvgPointSpreadLossesId { get; set; }
        public string LeastAvgPointSpreadLosses { get; set; }

        //string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public GlobalGameStats()
        {
            this._repo = new GenericRespository(_db);
             
            this.globalStats = this.LoadGlobalStats();

            this.PlayerWithMostWins = globalStats.PlayerWithMostWins;
            this.PlayerWithMostWinsId = globalStats.PlayerWithMostWinsId;
            this.MostWins = globalStats.MostWins.ToString();

            this.PlayerWithMostGames = globalStats.PlayerWithMostWins;
            this.PlayerWithMostGamesId = globalStats.PlayerWithMostGamesId;
            this.MostGames = globalStats.MostWins.ToString();

            this.GameWithLongestVolleyHits = globalStats.GameWithLongestVolleyHits;
            this.Player1GameWithLongestVolleyHits = globalStats.Player1GameWithLongestVolleyHits;
            this.Player1GameWithLongestVolleyHitsId = globalStats.Player1GameWithLongestVolleyHitsId;
            this.Player2GameWithLongestVolleyHits = globalStats.Player2GameWithLongestVolleyHits;
            this.Player2GameWithLongestVolleyHitsId = globalStats.Player2GameWithLongestVolleyHitsId;
            this.LongestVolleyHits = globalStats.LongestVolleyHits.ToString();

            this.GameWtihLongestVolleyTime = globalStats.GameWithLongestVolleyTime;
            this.Player1GameWithLongestVolleyTime = globalStats.Player1GameWithLongestVolleyTime;
            this.Player1GameWithLongestVolleyTimeId = globalStats.Player1GameWithLongestVolleyHitsId;
            this.Player2GameWithLongestVolleyTime = globalStats.Player2GameWithLongestVolleyTime;
            this.Player2GameWithLongestVolleyTimeId = globalStats.Player2GameWithLongestVolleyTimeId;
            this.LongestVolleyTime = globalStats.LongestVolleyTime.ToString("0.##");

            this.PlayerWithBestWinRatio = globalStats.PlayerWithBestWinRatio;
            this.PlayerWithBestWinRatioId = globalStats.PlayerWithBestWinRatioId;
            this.BestWinRatio = globalStats.BestWinRatio.ToString("0.##");

            this.PlayerWithGreatestAvgPointSpreadWins = globalStats.PlayerWithGreatestAvgPointSpreadWins;
            this.PlayerWithGreatestAvgPointSpreadWinsId = globalStats.PlayerWithGreatestAvgPointSpreadWinsId;
            this.BestAvgPointSpreadWins = globalStats.BestAvgPointSpreadWins.ToString("0.##");

            this.PlayerWithLeastAvgPointSpreadLosses = globalStats.PlayerWithLeastAvgPointSpreadLosses;
            this.PlayerWithLeastAvgPointSpreadLossesId = globalStats.PlayerWithLeastAvgPointSpreadLossesId;
            this.LeastAvgPointSpreadLosses = globalStats.LeastAvgPointSpreadLosses.ToString("0.##");
        }

        private GlobalGameStatsView LoadGlobalStats()
        {
            GlobalGameStatsView globalStats = (from g in _repo.Query<GlobalStats>()
                                               orderby g.Id descending
                                               select new GlobalGameStatsView
                                               {
                                                   Id = g.Id,
                                                   PlayerWithMostWins = g.PlayerWithMostWins,
                                                   PlayerWithMostGamesId = g.PlayerWithMostGamesId,
                                                   MostWins = g.MostWins,
                                                   PlayerWithMostGames = g.PlayerWithMostGames,
                                                   PlayerWithMostWinsId = g.PlayerWithMostWinsId,
                                                   MostGamesPlayed = g.MostGamesPlayed,
                                                   PlayerWithBestWinRatio = g.PlayerWithBestWinRatio,
                                                   PlayerWithBestWinRatioId = g.PlayerWithBestWinRatioId,
                                                   BestWinRatio = g.BestWinRatio,
                                                   PlayerWithGreatestAvgPointSpreadWins = g.PlayerWithGreatestAvgPointSpreadWins,
                                                   PlayerWithGreatestAvgPointSpreadWinsId = g.PlayerWithGreatestAvgPointSpreadWinsId,
                                                   BestAvgPointSpreadWins = g.BestAvgPointSpreadWins,
                                                   PlayerWithLeastAvgPointSpreadLosses = g.PlayerWithLeastAvgPointSpreadLosses,
                                                   PlayerWithLeastAvgPointSpreadLossesId = g.PlayerWithLeastAvgPointSpreadLossesId,
                                                   LeastAvgPointSpreadLosses = g.LeastAvgPointSpreadLosses,
                                                   GameWithLongestVolleyHits = g.GameWithLongestVolleyHits,
                                                   Player1GameWithLongestVolleyHits = g.Player1GameWithLongestVolleyHits,
                                                   Player1GameWithLongestVolleyHitsId = g.Player1GameWithLongestVolleyHitsId,
                                                   Player2GameWithLongestVolleyHits = g.Player2GameWithLongestVolleyHits,
                                                   Player2GameWithLongestVolleyHitsId = g.Player2GameWithLongestVolleyHitsId,
                                                   LongestVolleyHits = g.LongestVolleyHits,
                                                   GameWithLongestVolleyTime = g.GameWithLongestVolleyTime,
                                                   Player1GameWithLongestVolleyTime = g.Player1GameWithLongestVolleyTime,
                                                   Player1GameWithLongestVolleyTimeId = g.Player1GameWithLongestVolleyTimeId,
                                                   Player2GameWithLongestVolleyTime = g.Player2GameWithLongestVolleyTime,
                                                   Player2GameWithLongestVolleyTimeId = g.Player2GameWithLongestVolleyTimeId,
                                                   LongestVolleyTime = g.LongestVolleyTime,
                                                   LastUpdated = g.LastUpdated
                                               }).FirstOrDefault();
            return globalStats;
        }
    }
}
