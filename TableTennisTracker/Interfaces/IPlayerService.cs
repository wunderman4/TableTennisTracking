using System.Collections.Generic;
using TableTennisTracker.Models;
using TableTennisTracker.ModelViews;

namespace TableTennisTracker.Interfaces
{
    public interface IPlayerService
    {
        void AddPlayer(Player newPlayer);
        void DeletePlayer(int id);
        Player GetPlayer(int id);
        string GetPlayerLongestVolley(int id);
        string GetPlayerLongestVolleyTime(int id);
        PlayerWithGames GetPlayerWithGames(int id);
        Player GetPlayerWithMostGames();
        Player GetPlayerWithMostLosses();
        Player GetPlayerWithMostWins();
        List<Player> ListPlayers();
        List<PlayerWithGames> ListPlayersWithGames();
        bool PlayerHasGames(int id);
        void UpdatePlayer(Player updatedPlayer);
    }
}