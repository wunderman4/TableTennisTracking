using System.Collections.Generic;
using TableTennisTracker.Models;
using TableTennisTracker.Views;

namespace TableTennisTracker.Interfaces
{
    public interface IGameService
    {
        void AddGame(Game newGame, List<HitLocation> bounces);
        void DeleteGame(int id);
        GamesView GetGame(int id);
        List<GamesView> GetGames();
        List<GamesView> GetPlayerGames(int id);
        void UpdateGame(Game updatedGame);
    }
}