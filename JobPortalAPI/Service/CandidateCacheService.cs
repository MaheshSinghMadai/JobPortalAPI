using JobPortalAPI.Data;
using JobPortalAPI.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace JobPortalAPI.Service
{
    public class CandidateCacheService(IMemoryCache _cache, ApplicationDbContext _db)
    {
        //create a cache key
        private const string CandidateListCacheKey = "CandidateList";
        public List<Candidate> GetAllCandidates()
        {
            if (!_cache.TryGetValue(CandidateListCacheKey, out List<Candidate> cachedCandidates))
            {
                //fetch candidates list from database
                cachedCandidates = _db.Candidates.AsNoTracking().ToList();

                //store candidates list into cache if cache is empty
                _cache.Set(CandidateListCacheKey, cachedCandidates, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10), 
                    //SlidingExpiration = TimeSpan.FromMinutes(5) 
                });
            }

            return cachedCandidates;
        }

        public Candidate GetCandidateByEmail(string email)
        {
            var candidates = GetAllCandidates();

            return candidates.FirstOrDefault(c => c.Email.Equals(email));
        }
    }
}
