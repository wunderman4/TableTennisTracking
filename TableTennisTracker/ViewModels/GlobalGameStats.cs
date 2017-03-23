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
        public Player PlayerWithMostLosses { get; set; }
        public GamesView GameWithLongestVolleyHits { get; set; }
        public GamesView GameWtihLongestVolleyTime { get; set; }
        public Player PlayerWithMostGames { get; set; }
        public Player PlayerWithGreatestWinRatio { get; set; }
        public Player PlayerWithGreatestAvgPointSpreadWins { get; set; }
        public Player PlayerWithGreatestAvgPointSpreadLosses { get; set; }

        public GlobalGameStats()
        {
            this.gs = new GameService();
            this.ps = new PlayerService();

            this.playersWtihGames = ps.ListPlayersWithGames();

            this.PlayerWithMostWins = ps.GetPlayerWithMostWins();
            this.PlayerWithMostLosses = ps.GetPlayerWithMostWins();
            this.PlayerWithMostGames = ps.GetPlayerWithMostGames();
            this.GameWithLongestVolleyHits = gs.GetGameWithLongestVolley();
            this.GameWtihLongestVolleyTime = gs.GetGameWithLongestVolleyTime();
        }

        private void GetPlayerWithGreatestWinRatio()
        {
            //Player playerWithBestWinRatio;

            var ratioList = new List<string>();

            foreach(PlayerWithGames player in playersWtihGames)
            {
                decimal totalGames = player.Wins + player.Losses;

                decimal ratio = player.Wins / totalGames;

                ratioList.Add(ratio.ToString() + " " + player.Id.ToString());
            }

            ratioList.Sort();

            //return playerWithBestWinRatio;
        }
    }
}
