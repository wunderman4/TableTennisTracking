using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableTennisTracker.Models
{
    public class GamePlayer
    {
        public Game Game { get; set; }
        public int GameId { get; set; }
        public Player Player { get; set; }
        public int PlayerId { get; set; }
    }
}
