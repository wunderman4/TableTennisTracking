using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTennisTracker.Models;
using TableTennisTracker.ModelViews;
using TableTennisTracker.Services;

namespace TableTennisTracker.ModelViews
{
    public class PlayerGameStats
    {
        private int id;

        private PlayerService ps;

        private PlayerWithGames playerWithGames;

        public string Wins { get; set; }
        public string Losses { get; set; }
        public string TotalGames { get; set; }
        public string WinLossRatio { get; set; }
        public string AvgPointSpreadWins { get; set; }
        public string AvgPointSpreadLosses { get; set; }
        public string LongestVolleyHits { get; set; }
        public string LongestVolleyTime { get; set; }

        public PlayerGameStats(int id)
        {
            this.id = id;
            this.ps = new PlayerService();
            
            this.playerWithGames = ps.GetPlayerWithGames(id);

            this.Wins = playerWithGames.Wins.ToString();
            this.Losses = playerWithGames.Losses.ToString();
            this.TotalGames = GetTotalGames();
            this.WinLossRatio = GetWinLossRatio();
            this.AvgPointSpreadWins = GetAvgPointSpreadWins();
            this.AvgPointSpreadLosses = GetAvgPointSpreadLosses();

            this.LongestVolleyHits = ps.GetPlayerLongestVolley(id);
            this.LongestVolleyTime = ps.GetPlayerLongestVolleyTime(id);
        }
        private string GetWinLossRatio()
        {
            decimal ratio;

            decimal totalGames = playerWithGames.Wins + playerWithGames.Losses;

            if(totalGames != 0)
            {
                ratio = playerWithGames.Wins / totalGames;
            }
            else
            {
                ratio = 0;
            }
 
            return ratio.ToString("0.###");
        }

        private string GetTotalGames()
        {
            decimal totalGames = playerWithGames.Wins + playerWithGames.Losses;

            return totalGames.ToString();
        }

        private string GetAvgPointSpreadWins()
        {
            decimal gamesWon = 0;

            decimal gameSpread = 0;

            decimal AvgPointSpread = 0;

            decimal totalGames = playerWithGames.Wins + playerWithGames.Losses;

            if (totalGames != 0)
            {
                foreach (Game game in playerWithGames.Games)
                {
                    if (game.Player1.Id == id)
                    {
                        if (game.Player1Score > game.Player2Score)
                        {
                            gameSpread += game.Player1Score - game.Player2Score;

                            gamesWon++;
                        }
                    }
                    else
                    {
                        if (game.Player2Score > game.Player1Score)
                        {
                            gameSpread += game.Player2Score - game.Player1Score;

                            gamesWon++;
                        }
                    }
                }

                if (gamesWon != 0)
                {
                    AvgPointSpread = gameSpread / gamesWon;
                }
                else
                {
                    AvgPointSpread = 0;
                }

            }
            else
            {
                AvgPointSpread = 0;
            }
            
            return AvgPointSpread.ToString("0.###");
        }

        private string GetAvgPointSpreadLosses()
        {
            decimal gamesLost = 0;

            decimal gameSpread = 0;

            decimal AvgPointSpread = 0;

            decimal totalGames = playerWithGames.Wins + playerWithGames.Losses;

            if (totalGames != 0)
            {

                foreach (Game game in playerWithGames.Games)
                {
                    if (game.Player1.Id == id)
                    {
                        if (game.Player2Score > game.Player1Score)
                        {
                            gameSpread += game.Player2Score - game.Player1Score;

                            gamesLost++;
                        }
                    }
                    else
                    {
                        if (game.Player1Score > game.Player2Score)
                        {
                            gameSpread += game.Player1Score - game.Player2Score;

                            gamesLost++;
                        }
                    }
                }

                if(gamesLost != 0)
                {
                    AvgPointSpread = gameSpread / gamesLost;
                }
                else
                {
                    AvgPointSpread = 0;
                }
            }
            else
            {
                AvgPointSpread = 0;
            }

            return AvgPointSpread.ToString("0.###");
        }
    }
}
