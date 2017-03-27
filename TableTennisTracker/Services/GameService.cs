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
using TableTennisTracker.ModelViews;

namespace TableTennisTracker.Services
{
    public class GameService : IGameService
    {
        private GenericRespository _repo;

        TableTennisTrackerDb _db = new TableTennisTrackerDb();

        private GamePlayerService _gamePlayerSer = new GamePlayerService();

        private HitLocationService _hitLocationSer = new HitLocationService();

        public GameService()
        {
            this._repo = new GenericRespository(_db);
        }

        /// <summary>
        /// GetGames() - returns all games
        /// </summary>
        /// <returns>List of GamesView</returns>
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
        
        /// <summary>
        /// GetPlayerGames(int player's id) - returns all games for a particular player
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of GamesView for a player</returns>
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

        /// <summary>
        /// GetGame(int game id) - returns a single game
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a single GamesView</returns>
        public GamesView GetGame(int id)
        {
            GamesView game;
            List<HitLocation> gameHitLocations;

            game = (from g in _repo.Query<Game>()
                      where g.Id == id
                      select new GamesView
                      {
                          Id = g.Id,
                          Player1Id = g.Player1.Id,
                          Player2Id = g.Player2.Id,
                          Player1Score = g.Player1Score,
                          Player2Score = g.Player2Score,
                          MaxVelocity = g.MaxVelocity,
                          LongestVolleyHits = g.LongestVolleyHits,
                          LongestVolleyTime = g.LongestVolleyTime,
                          Player1 = g.Player1,
                          Player2 = g.Player2,
                      }).FirstOrDefault();

            gameHitLocations = (from h in _repo.Query<HitLocation>()
                                where h.Game.Id == game.Id
                                select h).ToList();

            game.GameHitLocations = gameHitLocations;

            return game;
        }

        public void AddGame(Game newGame, List<HitLocation> bounces)
        {
            Player player1 = (from p in _repo.Query<Player>()
                              where p.Id == newGame.Player1.Id
                              select p).FirstOrDefault();

            Player player2 = (from p in _repo.Query<Player>()
                              where p.Id == newGame.Player2.Id
                              select p).FirstOrDefault();

            if(newGame.Player1Score > newGame.Player2Score)
            {
                player1.Wins++;
                player2.Losses++;
            }
            else
            {
                player1.Losses++;
                player2.Wins++;
            }

            _db.SaveChanges();

            newGame.Player1 = player1;
            newGame.Player2 = player2;

            DateTime myDateTime = DateTime.Now;
            
            newGame.CreatedDate = myDateTime;

            // Add the game to the Games table:
            _repo.Add(newGame);

            // Add the players to the GamePlayer table:
            _gamePlayerSer.AddGamePlayers(newGame);

            // Add the hit locations (bounces) to the HitLocation table:

            Game currentGame = new Game
            {
                Id = newGame.Id
            };

            foreach (HitLocation hitLocation in bounces)
            {
                hitLocation.Game = currentGame;
                _hitLocationSer.AddHitLocation(hitLocation);
            }
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
            Player player1,player2;

            Game gameToBeDeleted = (from g in _repo.Query<Game>()
                                    where g.Id == id
                                    select g).FirstOrDefault();

            GamesView gameData = GetGame(id);

            player1 = (from p in _repo.Query<Player>()
                      where p.Id == gameData.Player1.Id
                      select p).FirstOrDefault();

            player2 = (from p in _repo.Query<Player>()
                       where p.Id == gameData.Player2.Id
                       select p).FirstOrDefault();

            // adjust the player's Win/Losses:

            if(gameToBeDeleted.Player1Score > gameToBeDeleted.Player2Score)
            {
                player1.Wins--;
                player2.Losses--;
            }
            else
            {
                player2.Wins--;
                player1.Losses--;
            }
            _db.SaveChanges();

            //delete the game's hit locations first:
            _hitLocationSer.DeleteHitLocationsGame(id);
            
            //now delete the game:
            _repo.Delete(gameToBeDeleted);
        }

        ///////////////Global Game Stats:////////////////////

        public GamesView GetGameWithLongestVolley()
        {
            Player player;

            GamesView game = (from g in _repo.Query<Game>()
                         orderby g.LongestVolleyHits descending
                         select new GamesView
                         {
                             Id = g.Id,
                             Player1Id = g.Player1.Id,
                             Player2Id = g.Player2.Id,
                             Player1Score = g.Player1Score,
                             Player2Score = g.Player2Score,
                             MaxVelocity = g.MaxVelocity,
                             LongestVolleyHits = g.LongestVolleyHits,
                             LongestVolleyTime = g.LongestVolleyTime,
                             Player1 = g.Player1,
                             Player2 = g.Player2
                         }).FirstOrDefault();

            player = (from p in _repo.Query<Player>()
                      where p.Id == game.Player1Id
                      select p).FirstOrDefault();
            game.Player1 = player;

            player = (from p in _repo.Query<Player>()
                      where p.Id == game.Player2Id
                      select p).FirstOrDefault();
            game.Player2 = player;

            return game;
        }

        public GamesView GetGameWithLongestVolleyTime()
        {
            Player player;

            GamesView game = (from g in _repo.Query<Game>()
                         orderby g.LongestVolleyTime descending
                         select new GamesView
                        {
                             Id = g.Id,
                             Player1Id = g.Player1.Id,
                             Player2Id = g.Player2.Id,
                             Player1Score = g.Player1Score,
                             Player2Score = g.Player2Score,
                             MaxVelocity = g.MaxVelocity,
                             LongestVolleyHits = g.LongestVolleyHits,
                             LongestVolleyTime = g.LongestVolleyTime,
                             Player1 = g.Player1,
                             Player2 = g.Player2
                         }).FirstOrDefault();

            player = (from p in _repo.Query<Player>()
                      where p.Id == game.Player1Id
                      select p).FirstOrDefault();
            game.Player1 = player;

            player = (from p in _repo.Query<Player>()
                      where p.Id == game.Player2Id
                      select p).FirstOrDefault();
            game.Player2 = player;

            return game;
        }

        /// <summary>
        /// GetGamesFor(int player1Id, int player2Id)
        /// Get all games for a particular pair of players
        /// </summary>
        /// <param name="player1Id"></param>
        /// <param name="player2Id"></param>
        /// <returns>Returns a list of GamesView(s) (without hit locations)</returns>
        public List<GamesView> GetGamesFor(int player1Id, int player2Id)
        {
            List<GamesView> gameList = (from g in _repo.Query<Game>()
                                        where g.Player1.Id == player1Id && g.Player2.Id == player2Id
                                        select new GamesView
                                        {
                                            Id = g.Id,
                                            Player1Id = g.Player1.Id,
                                            Player2Id = g.Player2.Id,
                                            Player1 = g.Player1,
                                            Player2 = g.Player2
                                        }).ToList();

            List<GamesView> gameList1 = (from g in _repo.Query<Game>()
                                        where g.Player1.Id == player2Id && g.Player2.Id == player1Id
                                        select new GamesView
                                        {
                                            Id = g.Id,
                                            Player1Id = g.Player1.Id,
                                            Player2Id = g.Player2.Id,
                                            Player1 = g.Player1,
                                            Player2 = g.Player2
                                        }).ToList();

            foreach(GamesView game in gameList1)
            {
                gameList.Add(game);
            }

            return gameList;
        }
    }
}
