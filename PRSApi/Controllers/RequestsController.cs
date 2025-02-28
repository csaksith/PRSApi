using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSApi.Models;

namespace PRSApi.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase {
        private readonly PRSContext _context;

        public RequestsController(PRSContext context) {
            _context=context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests() {
            var requests = _context.Requests.Include(r => r.User);
            return await requests.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id) {
            var request = await _context.Requests.Include(r => r.User)
                                                 .FirstOrDefaultAsync(r => r.Id==id);

            if (request==null) {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id,Request request) {
            if (id!=request.Id) {
                return BadRequest();
            }
            request.Status="NEW";

            _context.Entry(request).State=EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request) {
            // automatically set new requests as "NEW"
            request.Status="NEW";
            request.Total=0;
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetRequest",new { id = request.Id },request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id) {
            var request = await _context.Requests.FindAsync(id);
            if (request==null) {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Requests/Users/2
        [HttpGet("users/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsbyUser(int userId) {
            var requests = _context.Requests.Include(r => r.User)
                                            .Where(r => r.UserId==userId);
            return await requests.ToListAsync();
        }

        private bool RequestExists(int id) {
            return _context.Requests.Any(e => e.Id==id);
        }

        // GET: api/Requests/list-review/7
        [HttpGet("list-review/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsForReview(int userId) {
            var requests = await _context.Requests
                .Where(r => r.Status=="REVIEW"&&r.UserId==userId)
                .ToListAsync();
            return Ok(requests);
        }

        // PUT: api/Requests/submit-review
        [HttpPut("submit-review/{id}")]
        public async Task<IActionResult> SubmitReview(int id) {
            // get request by id
            var request = await _context.Requests.FindAsync(id);
            if (request==null) {
                return NotFound("Request not found.");
            }
            // prevent duplicate submissions
            if (request.Status!="NEW") {
                return BadRequest("Request already submitted.");
            }

            // request status automatically approved if request total is less than $50
            request.Status=request.Total<=50 ? "APPROVED" : "REVIEW";
            request.SubmittedDate=DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(request);
        }
    }
}