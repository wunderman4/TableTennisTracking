using System.Collections.Generic;
using TableTennisTracker.Models;

namespace TableTennisTracker.Interfaces
{
    public interface IPlayerService
    {
        void AddPlayer(Player newPlayer);
        void DeletePlayer(int id);
        void Dispose();
        Player GetPlayer(int id);
        List<Player> ListPlayers();
        void UpdatePlayer(Player newPlayer);
    }
}