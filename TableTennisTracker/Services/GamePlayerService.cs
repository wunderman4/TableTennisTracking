using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTennisTracker.Interfaces;
using TableTennisTracker.Models;
using TableTennisTracker.Respository;

namespace TableTennisTracker.Services
{
    public class GamePlayerService : IGamePlayerService
    {
        TableTennisTrackerDb _db = new TableTennisTrackerDb();

        private GenericRespository _repo;

        public GamePlayerService()
        {
            this._repo = new GenericRespository(_db);
        }

        public void AddGamePlayers(Game newGame)
        {
            _repo.Add(new GamePlayer { GameId = newGame.Id, PlayerId = newGame.Player1.Id });
            _repo.Add(new GamePlayer { GameId = newGame.Id, PlayerId = newGame.Player2.Id });
        }
    }
}
