﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableTennisTracker.Interfaces;
using TableTennisTracker.Models;
using TableTennisTracker.Respository;

namespace TableTennisTracker.Services
{
    public class HitLocationService : IHitLocationService
    {
        TableTennisTrackerDb _db = new TableTennisTrackerDb();

        private GenericRespository _repo;

        public HitLocationService()
        {
            this._repo = new GenericRespository(_db);
        }
        /// <summary>
        /// GetHitLocations() - returns all hit locations
        /// </summary>
        /// <returns> List of HitLocation</returns>
        public List<HitLocation> GetHitLocations()
        {
            List<HitLocation> hitLocationList = (from h in _repo.Query<HitLocation>()
                                                 select h).ToList();
            return hitLocationList;
        }
        /// <summary>
        /// GetHitLocationsGame(int id) - returns hit locations for the
        ///                               passed in game id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List of HitLocation</returns>
        public List<HitLocation> GetHitLocationsGame(int id)
        {
            List<HitLocation> hitLocationList = (from h in _repo.Query<HitLocation>()
                                                 where h.Game.Id == id
                                                 select h).ToList();
            return hitLocationList;
        }
        /// <summary>
        /// GetHitLocationsGameVolley(int id, int volleyNbr) - 
        /// returns all the hit locations for a particular game and volley ids
        /// </summary>
        /// <param name="id"></param>
        /// <param name="volleyNbr"></param>
        /// <returns></returns>
        public List<HitLocation> GetHitLocationsGameVolley(int id, int volleyNbr)
        {
            List<HitLocation> hitLocationList = (from h in _repo.Query<HitLocation>()
                                                 where h.Game.Id == id && h.Volley == volleyNbr
                                                 select h).ToList();
            return hitLocationList;
        }

        public HitLocation GetSingleHitLocation(int id)
        {
            return (from h in _repo.Query<HitLocation>()
                    where h.Id == id
                    select h).FirstOrDefault();
        }

        public void AddHitLocation(HitLocation newHitLocation)
        {
            Game currentGame = (from g in _repo.Query<Game>()
                                where g.Id == newHitLocation.Game.Id
                                select g).FirstOrDefault();

            newHitLocation.Game = currentGame;

            _repo.Add(newHitLocation);
        }

        public void DeleteHitLocation(int id)
        {
            HitLocation hitLocationToBeDeleted = (from h in _repo.Query<HitLocation>()
                                                  where h.Id == id
                                                  select h).FirstOrDefault();
            _repo.Delete(hitLocationToBeDeleted);
        }

        public void DeleteHitLocationsGame(int id)
        {
            List<HitLocation> hitLocationList = (from h in _repo.Query<HitLocation>()
                                                 where h.Game.Id == id
                                                 select h).ToList();

            foreach(HitLocation hitLocation in hitLocationList)
            {
                this.DeleteHitLocation(hitLocation.Id);
            }
        }

        public void UpdateHitLocation(HitLocation updatedHitLocation)
        {
            HitLocation originalHitLocation = (from h in _repo.Query<HitLocation>()
                                               where h.Id == updatedHitLocation.Id
                                               select h).FirstOrDefault();

            originalHitLocation.X = updatedHitLocation.X;
            originalHitLocation.Y = updatedHitLocation.Y;
            originalHitLocation.Z = updatedHitLocation.Z;
            originalHitLocation.Volley = updatedHitLocation.Volley;
            originalHitLocation.Game.Id = updatedHitLocation.Game.Id;

            _repo.SaveChanges();
        }
    }
}
