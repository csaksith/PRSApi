using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSApi.Models;

namespace PRSApi.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class LineItemsController : ControllerBase {
        private readonly PRSContext _context;

        public LineItemsController(PRSContext context) {
            _context=context;
        }

        // GET: api/LineItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineItem>>> GetLineItems() {
            var lineitems = _context.LineItems.Include(l => l.Product)
                                              .Include(l => l.Request);
            return await lineitems.ToListAsync();
        }

        // GET: api/LineItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LineItem>> GetLineItem(int id) {
            var lineitem = await _context.LineItems.Include(l => l.Product)
                                                    .Include(l => l.Request)
                                                    .FirstOrDefaultAsync(l => l.Id==id);

            if (lineitem==null) {
                return NotFound();
            }

            return lineitem;
        }

        // PUT: api/LineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLineItem(int id,LineItem updatedLineItem) {
            var lineItem = await _context.LineItems.FindAsync(id);
            if (id==null) {
                return NotFound();
            }

            lineItem.Quantity=updatedLineItem.Quantity;
            lineItem.ProductId=updatedLineItem.ProductId;

            await _context.SaveChangesAsync();
            MyConsole.PrintLine($"Updated Line Item: {id}, RequestId: {lineItem.RequestId}");

            await _context.RecalcTotal(lineItem.RequestId);

            return Ok(lineItem);
        }

        // POST: api/LineItems
        [HttpPost]
        public async Task<ActionResult<LineItem>> PostLineItem(LineItem lineItem) {
            var product = await _context.Products.FindAsync(lineItem.ProductId);
            if (product==null) {
                return BadRequest($"Error: Product Id: {lineItem.ProductId} not found.");
            }

            _context.LineItems.Add(lineItem);
            await _context.SaveChangesAsync();
            MyConsole.PrintLine($"Added Line Item: {lineItem.Id}, RequestId: {lineItem.RequestId}");

            await _context.RecalcTotal(lineItem.RequestId);

            return Ok(lineItem);
        }

        // DELETE: api/LineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLineItem(int id) {
            var lineItem = await _context.LineItems.FindAsync(id);
            if (lineItem==null) {
                return NotFound();
            }

            int requestId = lineItem.RequestId;

            _context.LineItems.Remove(lineItem);
            await _context.SaveChangesAsync();

            MyConsole.PrintLine($"Deleted Line Item: {id}, RequestId: {requestId}");
            await _context.RecalcTotal(requestId);
            return NoContent();
        }

        public bool LineItemExists(int id) {
            return _context.LineItems.Any(e => e.Id==id);
        }
    }
}