using System.Collections.Generic;
using TableTennisTracker.Models;
using TableTennisTracker.Views;

namespace TableTennisTracker.Interfaces
{
    public interface IGameService
    {
        void AddGame(Game newGame);
        void DeleteGame(int id);
        Game GetGame(int id);
        List<GamesView> GetGames();
        List<GamesView> GetPlayerGames(int id);
        void UpdateGame(Game updatedGame);
    }
}