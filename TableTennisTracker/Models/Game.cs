using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTennisTracker.Models
{
    public class Game
    {
        public int Id { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public int Player1Score { get; set; }
        public int Player2Score { get; set; }
        public float MaxVelocity { get; set; }
        public float LongestVolleyTime { get; set; }
        public int LongestVolleyHits { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<HitLocation> GameHitLocations { get; set; }

        public ICollection<GamePlayer> GamePlayer { get; set; }
    }
}
