using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTennisTracker.Interfaces;
using TableTennisTracker.Models;
using TableTennisTracker.Respository;
using TableTennisTracker.ModelViews;

namespace TableTennisTracker.Services
{
    public class PlayerService : IPlayerService
    {
        private GenericRespository _repo;

        private GameService gs = new GameService();

        private TableTennisTrackerDb _db = new TableTennisTrackerDb();

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

        public bool PlayerHasGames(int id)
        {
            List<Game> games = (from gp in _repo.Query<GamePlayer>()
                                where gp.PlayerId == id
                                select gp.Game).ToList();
            if (games.Count() == 0)
                return false;
            else
                return true;
        }
        public List<PlayerWithGames> ListPlayersWithGames()
        {
            List<PlayerWithGames> playersWtihGames = (from p in _repo.Query<Player>()
                                                      select new PlayerWithGames
                                                      {
                                                          Id = p.Id,
                                                          UserName = p.UserName,
                                                          PlayerName = p.PlayerName,
                                                          Age = p.Age,
                                                          HeightFt = p.HeightFt,
                                                          HeightInch = p.HeightInch,
                                                          Nationality = p.Nationality,
                                                          HandPreference = p.HandPreference,
                                                          IsSelected = p.IsSelected,
                                                          Wins = p.Wins,
                                                          Losses = p.Losses
                                                      }).ToList();

            foreach(PlayerWithGames player in playersWtihGames)
            {
                List<Game> games = (from gp in _repo.Query<GamePlayer>()
                                    where gp.PlayerId == player.Id
                                    select gp.Game).ToList();

                foreach (Game game in games)
                {
                    GamesView gameData = gs.GetGame(game.Id);
                    game.Player1 = gameData.Player1;
                    game.Player2 = gameData.Player2;
                }

                player.Games = games;
            }

            return playersWtihGames;
        }
        public Player GetPlayer(int id)
        {
            return (from p in _repo.Query<Player>()
                    where p.Id == id
                    select p).FirstOrDefault();
        }

        public PlayerWithGames GetPlayerWithGames(int id)
        {
            PlayerWithGames playerWithGames = (from p in _repo.Query<Player>()
                                               where p.Id == id
                                               select new PlayerWithGames
                                               {
                                                   Id = p.Id,
                                                   UserName = p.UserName,
                                                   PlayerName = p.PlayerName,
                                                   Age = p.Age,
                                                   HeightFt = p.HeightFt,
                                                   HeightInch = p.HeightInch,
                                                   Nationality = p.Nationality,
                                                   HandPreference = p.HandPreference,
                                                   IsSelected = p.IsSelected,
                                                   Wins = p.Wins,
                                                   Losses = p.Losses
                                               }).FirstOrDefault();

            List<Game> games = (from gp in _repo.Query<GamePlayer>()
                          where gp.PlayerId == id
                          select gp.Game).ToList();

            foreach(Game game in games)
            {
                GamesView gameData = gs.GetGame(game.Id);
                game.Player1 = gameData.Player1;
                game.Player2 = gameData.Player2;
            }

            playerWithGames.Games = games;

            return playerWithGames;
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
            originalPlayer.Wins = updatedPlayer.Wins;
            originalPlayer.Losses = updatedPlayer.Losses;

            _repo.SaveChanges();
        }

        public void DeletePlayer(int id)
        {
            Player playerToBeDeleted = (from p in _repo.Query<Player>()
                                        where p.Id == id
                                        select p).FirstOrDefault();
            _repo.Delete(playerToBeDeleted);
        }

        ///////////////Individual Player Stats:////////////////////

        public string GetPlayerLongestVolley(int id)
        {
            int zero = 0;

            Game playerGame = (from p in _repo.Query<Player>()
                               join gp in _repo.Query<GamePlayer>() on p.Id equals gp.PlayerId
                               join g in _repo.Query<Game>() on gp.GameId equals g.Id
                               where p.Id == id
                               orderby g.LongestVolleyHits descending
                               select g).FirstOrDefault();

            if (playerGame == null)
            {
                return zero.ToString();
            }
            else
            {
                return playerGame.LongestVolleyHits.ToString();
            }
        }

        public string GetPlayerLongestVolleyTime(int id)
        {
            int zero = 0;

            Game playerGame = (from p in _repo.Query<Player>()
                               join gp in _repo.Query<GamePlayer>() on p.Id equals gp.PlayerId
                               join g in _repo.Query<Game>() on gp.GameId equals g.Id
                               where p.Id == id
                               orderby g.LongestVolleyTime descending
                               select g).FirstOrDefault();

            if (playerGame == null)
            {
                return zero.ToString("0.## Seconds");
            }
            else
            {
                return playerGame.LongestVolleyTime.ToString("0.## Seconds");
            }
        }

        ///////////////Global Player Stats:////////////////////

        public Player GetPlayerWithMostWins()
        {
            Player player = (from p in _repo.Query<Player>()
                             orderby p.Wins descending
                             select p).FirstOrDefault();
            return player;
        }

        public Player GetPlayerWithMostLosses()
        {
            Player player = (from p in _repo.Query<Player>()
                             orderby p.Losses descending
                             select p).FirstOrDefault();
            return player;
        }

        public Player GetPlayerWithMostGames()
        {
            Player player = (from p in _repo.Query<Player>()
                             orderby p.Wins + p.Losses descending
                             select p).FirstOrDefault();
            return player;
        }
    }
}
