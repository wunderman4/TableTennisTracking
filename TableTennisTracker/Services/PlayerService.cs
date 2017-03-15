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
    public class PlayerService : IPlayerService
    {
        TableTennisTrackerDb _db = new TableTennisTrackerDb();

        private GenericRespository _repo;

        public PlayerService()
        {
            this._repo = new GenericRespository(_db);
        }

        public List<Player> ListPlayers()
        {
            List<Player> playerList = (from p in _repo.Query<Player>()
                                       select p).ToList();

            return playerList;
        }

        public Player GetPlayer(int id)
        {
            return (from p in _repo.Query<Player>()
                    where p.Id == id
                    select p).FirstOrDefault();
        }

        public void AddPlayer(Player newPlayer)
        {
            _repo.Add(newPlayer);
        }

        public void UpdatePlayer(Player updatedPlayer)
        {
            Player originalPlayer = (from p in _repo.Query<Player>()
                                     where p.Id == updatedPlayer.Id
                                     select p).FirstOrDefault();

            originalPlayer.UserName = updatedPlayer.UserName;
            originalPlayer.PlayerName = updatedPlayer.PlayerName;
            originalPlayer.HeightFt = updatedPlayer.HeightFt;
            originalPlayer.HeightInch = updatedPlayer.HeightInch;
            originalPlayer.Nationality = updatedPlayer.Nationality;
            originalPlayer.HandPreference = updatedPlayer.HandPreference;
            originalPlayer.IsSelected = updatedPlayer.IsSelected;

            _repo.SaveChanges();
        }

        public void DeletePlayer(int id)
        {
            Player playerToBeDeleted = (from p in _repo.Query<Player>()
                                        where p.Id == id
                                        select p).FirstOrDefault();
            _repo.Delete(playerToBeDeleted);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
