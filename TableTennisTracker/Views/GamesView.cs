using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTennisTracker.Models;

namespace TableTennisTracker.Views
{
    public class GamesView
    {

        public int Id { get; set; }
        public Player Player1 { get; set; }
        public int Player1Id { get; set; }
        public Player Player2 { get; set; }
        public int Player2Id { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public float MaxVelocity { get; set; }
        public float LongestVolleyTime { get; set; }
        public int LongestVolleyHits { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public List<HitLocation> GameHitLocations { get; set; }

        public List<GamePlayer> GamePlayer { get; set; }

    }
}
