﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTennisTracker.ViewModels
{
    public class GlobalGameStatsView
    {
        public int Id { get; set; }
        public string PlayerWithMostWins { get; set; }
        public int PlayerWithMostWinsId { get; set; }
        public int MostWins { get; set; }
        public string PlayerWithMostGames { get; set; }
        public int PlayerWithMostGamesId { get; set; }
        public int MostGamesPlayed { get; set; }
        public string PlayerWithBestWinRatio { get; set; }
        public int PlayerWithBestWinRatioId { get; set; }
        public float BestWinRatio { get; set; }
        public string PlayerWithGreatestAvgPointSpreadWins { get; set; }
        public int PlayerWithGreatestAvgPointSpreadWinsId { get; set; }
        public float BestAvgPointSpreadWins { get; set; }
        public string PlayerWithLeastAvgPointSpreadLosses { get; set; }
        public int PlayerWithLeastAvgPointSpreadLossesId { get; set; }
        public float LeastAvgPointSpreadLosses { get; set; }
        public int GameWithLongestVolleyHits { get; set; }
        public string Player1GameWithLongestVolleyHits { get; set; }
        public int Player1GameWithLongestVolleyHitsId { get; set; }
        public string Player2GameWithLongestVolleyHits { get; set; }
        public int Player2GameWithLongestVolleyHitsId { get; set; }
        public int LongestVolleyHits { get; set; }
        public int GameWithLongestVolleyTime { get; set; }
        public string Player1GameWithLongestVolleyTime { get; set; }
        public int Player1GameWithLongestVolleyTimeId { get; set; }
        public string Player2GameWithLongestVolleyTime { get; set; }
        public int Player2GameWithLongestVolleyTimeId { get; set; }
        public float LongestVolleyTime { get; set; }
        public DateTime? LastUpdated { get; set; }


        public GlobalGameStatsView()
        {
            PlayerWithMostWins = "N/A";
            PlayerWithMostWinsId = 0;
            MostWins = 0;
            PlayerWithMostGames = "N/A";
            PlayerWithMostGamesId = 0;
            MostGamesPlayed = 0;
            PlayerWithBestWinRatio = "N/A";
            PlayerWithBestWinRatioId = 0;
            BestWinRatio = 0;
            PlayerWithGreatestAvgPointSpreadWins = "N/A";
            PlayerWithGreatestAvgPointSpreadWinsId = 0;
            BestAvgPointSpreadWins = 0;
            PlayerWithLeastAvgPointSpreadLosses = "N/A";
            PlayerWithLeastAvgPointSpreadLossesId = 0;
            LeastAvgPointSpreadLosses = 0;
            GameWithLongestVolleyHits = 0;
            Player1GameWithLongestVolleyHits = "N/A";
            Player1GameWithLongestVolleyHitsId = 0;
            Player2GameWithLongestVolleyHits = "N/A";
            Player2GameWithLongestVolleyHitsId = 0;
            LongestVolleyHits = 0;
            GameWithLongestVolleyTime = 0;
            Player1GameWithLongestVolleyTime = "N/A";
            Player1GameWithLongestVolleyTimeId = 0;
            Player2GameWithLongestVolleyTime = "N/A";
            Player2GameWithLongestVolleyTimeId = 0;
            LongestVolleyTime = 0;
        }
    }
}
