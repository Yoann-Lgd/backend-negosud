using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend_negosud.Entities;
using backend_negosud.Services;
using Microsoft.EntityFrameworkCore;

namespace backend_negosud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly StockService _stockService;
        private readonly PostgresContext _context;

        public StocksController(StockService stockService, PostgresContext context)
        {
            _stockService = stockService;
            _context = context;
        }

        // GET: api/stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAllStocks()
        {
            var stocks = await _context.Stocks.ToListAsync();
            return Ok(stocks);
        }

        // GET: api/stocks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Stock>> GetStockById(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock);
        }

        // POST: api/stocks
        [HttpPost]
        public async Task<ActionResult<Stock>> CreateStock([FromBody] Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _stockService.AddArticleToStock(
                stock.ArticleId,
                stock.Quantite,
                stock.RefLot,
                stock.SeuilMinimum,
                stock.ReapprovisionnementAuto
            );

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return CreatedAtAction(nameof(GetStockById), new { id = stock.StockId }, stock);
        }

        // PUT: api/stocks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] Stock stock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != stock.StockId)
            {
                return BadRequest("ID de stock incorrect.");
            }

            var result = await _stockService.UpdateStockQuantity(
                stock.StockId,
                stock.Quantite,
                1, // ID de l'utilisateur système (à adapter)
                "Mise à jour manuelle"
            );

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return NoContent();
        }

        // DELETE: api/stocks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            _context.Stocks.Remove(stock);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/stocks/{id}/history
        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<Inventorier>>> GetStockHistory(int id)
        {
            var result = await _stockService.GetStockHistory(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
