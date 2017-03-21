using TableTennisTracker.Models;

namespace TableTennisTracker.Interfaces
{
    public interface IGamePlayerService
    {
        void AddGamePlayers(Game newGame);
    }
}