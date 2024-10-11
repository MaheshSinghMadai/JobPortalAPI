using JobPortalAPI.Data;
using JobPortalAPI.Entity;
using JobPortalAPI.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CandidateController(
        ApplicationDbContext _db, 
        CandidateCacheService _candidateService,
        ILogger _logger) : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<Candidate>> AddOrUpdateCandidateInformation(Candidate candidate)
        {
            try
            {
                if(candidateExists(candidate.Email))
                {
                
                    _db.Update(candidate);
                    await _db.SaveChangesAsync();
                }
                else 
                {
                    _db.Add(candidate);
                    await _db.SaveChangesAsync();
                }

                return Ok(candidate);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error occurred.");
                return StatusCode(500, "An error occurred while updating the database. Please try again.");
            }
        }

        //for caching implementation
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAllCandidates()
        {
            var candidates = _candidateService.GetAllCandidates();
            return Ok(candidates);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetCandidateByEmail(string email)
        {
            var candidate = _candidateService.GetCandidateByEmail(email);
            if (candidate == null)
            {
                return NotFound("Candidate not found");
            }

            return Ok(candidate);
        }

        private bool candidateExists(string email)
        {
            return _db.Candidates.Any(x => x.Email == email);
        }
    }
}
