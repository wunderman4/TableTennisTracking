using System.Collections.Generic;
using TableTennisTracker.Models;

namespace TableTennisTracker.Interfaces
{
    public interface IHitLocationService
    {
        void DeleteHitLocation(int id);
        void DeleteHitLocationsGame(int id);
        List<HitLocation> GetHitLocations();
        List<HitLocation> GetHitLocationsGame(int id);
        HitLocation GetSingleHitLocation(int id);
        void UpdateHitLocation(HitLocation updatedHitLocation);
    }
}