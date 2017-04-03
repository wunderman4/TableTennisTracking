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
    public class GlobalGameStats
    {
        private GameService gs;

        private PlayerService ps;

        private List<PlayerWithGames> playersWtihGames;

        public Player PlayerWithMostWins { get; set; }
        public string MostWins { get; set; }

        public Player PlayerWithMostLosses { get; set; }
        public string MostLosses { get; set; }

        public GamesView GameWithLongestVolleyHits { get; set; }
        public string LongestVolleyHits { get; set; }

        public GamesView GameWtihLongestVolleyTime { get; set; }
        public string LongestVolleyTime { get; set; }

        public Player PlayerWithMostGames { get; set; }
        public string MostGames { get; set; }

        public Player PlayerWithBestWinRatio { get; set; }
        public string BestWinRatio { get; set; }

        public Player PlayerWithGreatestAvgPointSpreadWins { get; set; }
        public string BestAvgPointSpreadWins { get; set; }

        public Player PlayerWithLeastAvgPointSpreadLosses { get; set; }
        public string LeastAvgPointSpreadLosses { get; set; }

        //string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        public GlobalGameStats()
        {
            this.gs = new GameService();
            this.ps = new PlayerService();

            this.playersWtihGames = ps.ListPlayersWithGames();

            this.PlayerWithMostWins = ps.GetPlayerWithMostWins();
            this.MostWins = PlayerWithMostWins.Wins.ToString();

            //this.PlayerWithMostLosses = ps.GetPlayerWithMostLosses();
            //this.MostLosses = PlayerWithMostLosses.Losses.ToString();

            this.PlayerWithMostGames = ps.GetPlayerWithMostGames();
            this.MostGames = (PlayerWithMostGames.Wins + PlayerWithMostGames.Losses).ToString();

            this.GameWithLongestVolleyHits = gs.GetGameWithLongestVolley();
            this.LongestVolleyHits = GameWithLongestVolleyHits.LongestVolleyHits.ToString();

            this.GameWtihLongestVolleyTime = gs.GetGameWithLongestVolleyTime();
            this.LongestVolleyTime = GameWtihLongestVolleyTime.LongestVolleyTime.ToString("0.##");

            this.PlayerWithBestWinRatio = this.GetPlayerWithBestWinRatio();
            
            this.PlayerWithGreatestAvgPointSpreadWins = this.GetPlayerWithGreatestAvgPointSpreadWins();
            this.PlayerWithLeastAvgPointSpreadLosses = this.GetPlayerWithGreatestAvgPointSpreadLosses();
        }

        private Player GetPlayerWithBestWinRatio()
        {
            Player playerWithBestWinRatio;

            var ratioList = new List<string>();

            // loop through all players
            // calculating their win ratios
            // and putting them into a list:

            foreach(PlayerWithGames player in playersWtihGames)
            {
                decimal totalGames = player.Wins + player.Losses;

                if(totalGames != 0 && totalGames >= 5) // Avoid any players who don't have any games
                {
                    decimal ratio = player.Wins / totalGames;

                    string playerRatio = ratio.ToString("0.##") + " : " + player.Id.ToString();

                    ratioList.Add(playerRatio);
                }
            }

            // sort and reverse the ratio list
            // to get a descending list:
            ratioList.Sort();
            ratioList.Reverse();
                        
            // Get the player:
            playerWithBestWinRatio = ps.GetPlayer( ParseOutTopPlayerId(ratioList));

            // Assign the metric:

            this.BestWinRatio = ParseOutTopPlayerMetric(ratioList);

            return playerWithBestWinRatio;
        }

        private Player GetPlayerWithGreatestAvgPointSpreadWins()
        {
            Player playerWithBestAvgPointSpread;

            var avgPointSpreadList = new List<string>();

            // loop through all players
            // calculating their average point
            // spread for wins
            // and putting them into a list:

            foreach (PlayerWithGames player in playersWtihGames)
            {
                int playerId = player.Id;

                decimal gamesWon = 0;

                decimal gameSpread = 0;

                decimal AvgPointSpread = 0;

                decimal totalGames = player.Wins + player.Losses;

                if (totalGames != 0 && totalGames >= 5) // Avoid any players who 
                {                                       // don't have any games or too few
                    foreach (Game game in player.Games)
                    {
                        if (game.Player1.Id == playerId)
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

                    AvgPointSpread = gameSpread / gamesWon;

                    string playerAvgPointSpread = AvgPointSpread.ToString("0.##") + " : " + playerId.ToString();

                    avgPointSpreadList.Add( playerAvgPointSpread );
                }
            }

            // sort and reverse the list
            // to get a descending list:
            avgPointSpreadList.Sort();
            avgPointSpreadList.Reverse();

            // Get the player:
            playerWithBestAvgPointSpread = ps.GetPlayer( ParseOutTopPlayerId(avgPointSpreadList));

            // Assign the best average point spread for wins:
            this.BestAvgPointSpreadWins = ParseOutTopPlayerMetric(avgPointSpreadList);

            return playerWithBestAvgPointSpread;
        }

        private Player GetPlayerWithGreatestAvgPointSpreadLosses()
        {
            Player playerWithBestAvgPointSpread;

            var avgPointSpreadList = new List<string>();

            // loop through all players
            // calculating their average point
            // spread for wins
            // and putting them into a list:

            foreach (PlayerWithGames player in playersWtihGames)
            {
                int playerId = player.Id;

                decimal gamesLost = 0;

                decimal gameSpread = 0;

                decimal AvgPointSpread = 0;

                decimal totalGames = player.Wins + player.Losses;

                if (totalGames != 0 && totalGames >= 5) // Avoid any players who 
                {                                       // don't have any games or too few
                    foreach (Game game in player.Games)
                    {
                        if (game.Player1.Id == playerId)
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

                    AvgPointSpread = gameSpread / gamesLost;

                    string playerAvgPointSpread = AvgPointSpread.ToString("0.##") + " : " + playerId.ToString();

                    avgPointSpreadList.Add(playerAvgPointSpread);
                }
            }

            // sort and reverse the list
            // to get a descending list:
            avgPointSpreadList.Sort();
            //avgPointSpreadList.Reverse();

            // Get the player:
            playerWithBestAvgPointSpread = ps.GetPlayer(ParseOutTopPlayerId(avgPointSpreadList));

            this.LeastAvgPointSpreadLosses = ParseOutTopPlayerMetric(avgPointSpreadList);

            return playerWithBestAvgPointSpread;
        }

        private int ParseOutTopPlayerId(List<string> list)
        {
            // Grab the top most entry:
            string top = list[0];

            // Split out the entry using colon as the delimiter:
            string[] parms = top.Split(':');

            // Grab the player's id (second parameter) and 
            // convert it to an int:
            int playerId = Convert.ToInt32(parms[1]);

            // return the player's id:
            return playerId;
        }

        private string ParseOutTopPlayerMetric(List<string> list)
        {
            // Grab the top most entry:
            string top = list[0];

            // Split out the entry using colon as the delimiter:
            string[] parms = top.Split(':');

            // Grab the player's metric (first parameter):
            string metric = parms[0];

            // return the player's id:
            return metric;
        }
    }
}
