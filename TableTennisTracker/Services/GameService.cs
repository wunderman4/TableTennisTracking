using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTennisTracker.Interfaces;
using TableTennisTracker.Models;
using TableTennisTracker.Respository;
using TableTennisTracker.Views;

namespace TableTennisTracker.Services
{
    public class GameService : IGameService
    {
        TableTennisTrackerDb _db = new TableTennisTrackerDb();

        private GenericRespository _repo;

        private HitLocationService _hitLocationSer = new HitLocationService();

        public GameService()
        {
            this._repo = new GenericRespository(_db);
        }

        public List<GamesView> GetGames()
        {
            int playerId;
            Player player;
            List<HitLocation> gameHitLocations;

            List<GamesView> gameList = (from g in _repo.Query<Game>()
                                    select new GamesView
                                    {
                                        Id = g.Id,
                                        Player1Id = g.Player1.Id,
                                        Player1Score = g.Player1Score,
                                        Player2Id = g.Player2.Id,
                                        Player2Score = g.Player2Score,
                                        MaxVelocity = g.MaxVelocity,
                                        LongestVolleyHits = g.LongestVolleyHits,
                                        LongestVolleyTime = g.LongestVolleyTime
                                    }).ToList();

            foreach(GamesView game in gameList)
            {
                playerId = game.Player1Id;

                player = (from p in _repo.Query<Player>()
                          where p.Id == playerId
                          select p).FirstOrDefault();
                game.Player1 = player;

                playerId = game.Player2Id;

                player = (from p in _repo.Query<Player>()
                          where p.Id == playerId
                          select p).FirstOrDefault();
                game.Player2 = player;

                gameHitLocations = (from h in _repo.Query<HitLocation>()
                                    where h.Game.Id == game.Id
                                    select h).ToList();

                game.GameHitLocations = gameHitLocations;
            }

            return gameList;
        }

        public List<GamesView> GetPlayerGames(int id)
        {
            int playerId;
            Player player;
            List<HitLocation> gameHitLocations;

            List<GamesView> gameList = (from g in _repo.Query<Game>()
                                    where g.Player1.Id == id || g.Player2.Id == id
                                    select new GamesView
                                    {
                                        Id = g.Id,
                                        Player1Id = g.Player1.Id,
                                        Player1Score = g.Player1Score,
                                        Player2Id = g.Player2.Id,
                                        Player2Score = g.Player2Score,
                                        MaxVelocity = g.MaxVelocity,
                                        LongestVolleyHits = g.LongestVolleyHits,
                                        LongestVolleyTime = g.LongestVolleyTime

                                    }).ToList();
           

            foreach (GamesView game in gameList)
            {
                playerId = game.Player1Id;

                player = (from p in _repo.Query<Player>()
                          where p.Id == playerId
                          select p).FirstOrDefault();
                game.Player1 = player;

                playerId = game.Player2Id;

                player = (from p in _repo.Query<Player>()
                          where p.Id == playerId
                          select p).FirstOrDefault();
                game.Player2 = player;

                gameHitLocations = (from h in _repo.Query<HitLocation>()
                                    where h.Game.Id == game.Id
                                    select h).ToList();

                game.GameHitLocations = gameHitLocations;
            }

            return gameList;
        }


        public Game GetGame(int id)
        {
            Game outGame;

           outGame = (from g in _repo.Query<Game>()
                      where g.Id == id
                      select g).FirstOrDefault();

            return outGame;
        }

        public void AddGame(Game newGame)
        {
            Player player1 = (from p in _repo.Query<Player>()
                              where p.Id == newGame.Player1.Id
                              select p).FirstOrDefault();

            Player player2 = (from p in _repo.Query<Player>()
                              where p.Id == newGame.Player2.Id
                              select p).FirstOrDefault();

            newGame.Player1 = player1;
            newGame.Player2 = player2;

            _repo.Add(newGame);

            _db.GamePlayers.Add(new GamePlayer { GameId = newGame.Id, PlayerId = player1.Id });
            _db.GamePlayers.Add(new GamePlayer { GameId = newGame.Id, PlayerId = player2.Id });
            _db.SaveChanges();
        }

        public void UpdateGame(Game updatedGame)
        {
            Game originalGame = (from g in _repo.Query<Game>()
                                 where g.Id == updatedGame.Id
                                 select g).FirstOrDefault();

            originalGame.Player1 = updatedGame.Player1;
            originalGame.Player2 = updatedGame.Player2;
            originalGame.Player1Score = updatedGame.Player1Score;
            originalGame.Player2Score = updatedGame.Player2Score;
            originalGame.MaxVelocity = updatedGame.MaxVelocity;
            originalGame.LongestVolleyTime = updatedGame.LongestVolleyTime;
            originalGame.LongestVolleyHits = updatedGame.LongestVolleyHits;

            _repo.SaveChanges();
        }

        public void DeleteGame(int id)
        {
            Game gameToBeDeleted = (from g in _repo.Query<Game>()
                                    where g.Id == id
                                    select g).FirstOrDefault();
            //delete the game's hit locations first:
            _hitLocationSer.DeleteHitLocationsGame(id);
            
            //now delete the game:
            _repo.Delete(gameToBeDeleted);
        }
    }
}
