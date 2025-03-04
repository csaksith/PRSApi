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

        // Generate Request Number Method
        private string getNextRequestNumber() {
            // requestNumber format: R2409230011
            // 11 chars, 'R' + YYMMDD + 4 digit # w/ leading zeros
            string requestNbr = "R";
            // add YYMMDD string
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            requestNbr+=today.ToString("yyMMdd");
            // get maximum request number from db
            string maxReqNbr = _context.Requests.Max(r => r.RequestNumber);
            String reqNbr = "";
            if (maxReqNbr!=null) {
                // get last 4 characters, convert to number
                String tempNbr = maxReqNbr.Substring(7);
                int nbr = Int32.Parse(tempNbr);
                nbr++;
                // pad w/ leading zeros
                reqNbr+=nbr;
                reqNbr=reqNbr.PadLeft(4,'0');
            }
            else {
                reqNbr="0001";
            }
            requestNbr+=reqNbr;
            return requestNbr;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetRequests() {
            var requests = await _context.Requests.Select(request => new RequestDTO {
                Id=request.Id,
                UserId=request.UserId,
                RequestNumber=request.RequestNumber,
                Description=request.Description,
                Justification=request.Justification,
                DateNeeded=request.DateNeeded,
                DeliveryMode=request.DeliveryMode,
                Status=request.Status,
                Total = request.Total
            })
                .ToListAsync();
            return Ok(requests);
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestDTO>> GetRequest(int id) {
            var request = await _context.Requests
                .Where(request => request.Id==id)
                .Select(request => new RequestDTO {
                    Id=request.Id,
                    UserId=request.UserId,
                    RequestNumber=request.RequestNumber,
                    Description=request.Description,
                    Justification=request.Justification,
                    DateNeeded=request.DateNeeded,
                    DeliveryMode=request.DeliveryMode,
                    Status=request.Status,
                    Total=request.Total
                }).FirstOrDefaultAsync();

            if (request==null) {
                return NotFound();
            }

            return Ok(request);
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RequestDTO>> PostRequest(RequestDTO requestDTO) {
            var request = new Models.Request {
                UserId=requestDTO.UserId,
                RequestNumber=getNextRequestNumber(), // Generates unique request number
                Description=requestDTO.Description,
                Justification=requestDTO.Justification,
                DateNeeded=requestDTO.DateNeeded,
                DeliveryMode=requestDTO.DeliveryMode,
                Status="NEW",
                Total=0,
                SubmittedDate=DateTime.Now,
                ReasonForRejection=null
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            // Return response with the generated ID
            return CreatedAtAction(nameof(GetRequest),new { id = request.Id },new {
                id = request.Id,
                userId = request.UserId,
                requestNumber=request.RequestNumber,
                description = request.Description, 
                justification = request.Justification,
                dateNeeded = request.DateNeeded,
                deliveryMode = request.DeliveryMode,
                status=request.Status,
                total=request.Total,
                submittedDate=request.SubmittedDate
            });
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id,RequestDTO requestDTO) {
            var request = await _context.Requests.FindAsync(id);
            if (request==null) {
                return NotFound("Request not found.");
            }

            // Update fields using DTO
            request.UserId=requestDTO.UserId;
            request.Description=requestDTO.Description;
            request.Justification=requestDTO.Justification;
            request.DateNeeded=requestDTO.DateNeeded;
            request.DeliveryMode=requestDTO.DeliveryMode;
            request.Status="NEW";

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/Requests/approve/{id}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveRequest(int id) {
            var request = await _context.Requests.FindAsync(id);
            if (request==null) {
                return NotFound("Request not found.");
            }
            request.Status="APPROVED";
            await _context.SaveChangesAsync();
            return Ok(request);
        }

        // PUT: api/Requests/reject/{id}
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectRequest(int id,[FromBody] RequestDTO requestDTO) {
            var request = await _context.Requests.FindAsync(id);
            if (request==null) {
                return NotFound("Request not found.");
            }

            request.Status="REJECTED";
            request.ReasonForRejection=requestDTO.ReasonForRejection;
            await _context.SaveChangesAsync();
            return Ok(request);
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
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetRequestsbyUser(int userId) {
            var requests = await _context.Requests
                .Where(request => request.UserId==userId)
                .Select(request => new RequestDTO {
                    UserId=request.UserId,
                    Description=request.Description,
                    Justification=request.Justification,
                    DateNeeded=request.DateNeeded,
                    DeliveryMode=request.DeliveryMode
                }).ToListAsync();

            return Ok(requests);
        }

        private bool RequestExists(int id) {
            return _context.Requests.Any(e => e.Id==id);
        }

        // GET: api/Requests/list-review/7
        [HttpGet("list-review/{userId}")]
        public async Task<ActionResult<IEnumerable<RequestDTO>>> GetRequestsForReview(int userId) {
            var user = await _context.Users.FindAsync(userId);
            // check if user exists
            if (user==null) {
                return NotFound("User not found");
            }

            // get all requests in review status and does not include reviewer's own request
            var requests = await _context.Requests
               .Where(r => r.Status=="REVIEW"&&r.UserId!=userId)
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