using JobPortalAPI.Data;
using JobPortalAPI.Entity;
using Microsoft.AspNetCore.Mvc;

namespace JobPortalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CandidateController(ApplicationDbContext _db) : ControllerBase
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
            catch (Exception ex)
            {
                return BadRequest(ex);   
            }
        }

        private bool candidateExists(string email)
        {
            return _db.Candidates.Any(x => x.Email == email);
        }
    }
}
