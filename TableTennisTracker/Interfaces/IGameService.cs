using System.Collections.Generic;
using TableTennisTracker.Models;
using TableTennisTracker.ModelViews;

namespace TableTennisTracker.Interfaces
{
    public interface IGameService
    {
        void AddGame(Game newGame, List<HitLocation> bounces);
        void DeleteGame(int id);
        GamesView GetGame(int id);
        List<GamesView> GetGames();
        GamesView GetGameWithLongestVolley();
        GamesView GetGameWithLongestVolleyTime();
        List<GamesView> GetPlayerGames(int id);
        void UpdateGame(Game updatedGame);
    }
}