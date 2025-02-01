using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Services;
using backend_negosud.Entities;
using backend_negosud.Repository;

namespace backend_negosud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IMapper _mapper;
        private readonly IStockRepository _repository;

        public StocksController(IStockService stockService, IMapper mapper, IStockRepository repository)
        {
            _stockService = stockService;
            _mapper = mapper;
            _repository = repository;
        }

        // GET: api/stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockSummaryDto>>> GetAllStocks()
        {
            var stocks = await _repository.GetAllAsync();
            var stockDtos = _mapper.Map<List<StockSummaryDto>>(stocks);
            return Ok(stockDtos);
        }

        // GET: api/stocks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StockDetailDto>> GetStockById(int id)
        {
            var stock = await _repository.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            var stockDto = _mapper.Map<StockDetailDto>(stock);
            return Ok(stockDto);
        }

        // POST: api/stocks
        [HttpPost]
        public async Task<ActionResult<StockSummaryDto>> CreateStock([FromBody] StockInputDto createStockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _stockService.AddArticleToStock(
                createStockDto.ArticleId,
                createStockDto.Quantite,
                createStockDto.RefLot,
                createStockDto.SeuilMinimum,
                createStockDto.ReapprovisionnementAuto
            );

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var stock = await _repository.GetByIdAsync(result.Data.StockId);
            var stockDto = _mapper.Map<StockSummaryDto>(stock);
            return CreatedAtAction(nameof(GetStockById), new { id = stock.StockId }, stockDto);
        }

        // PUT: api/stocks/{id}
        /// <summary>
        /// Attention, permet seulement l'update de la quantité, le reste ne sera pas affecté
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStockQuntity(int id, [FromBody] StockUpdateDto updateStockDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stock = await _repository.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            var result = await _stockService.UpdateStockQuantity(
                id,
                updateStockDto.Quantite,
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
            var stock = await _repository.GetByIdAsync(id);
            if (stock == null)
            {
                return NotFound();
            }

            await _repository.DeleteAsync(stock);
            return NoContent();
        }

        // GET: api/stocks/{id}/history
        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<StockHistoryDto>>> GetStockHistory(int id)
        {
            var result = await _stockService.GetStockHistory(id);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            var historyDtos = _mapper.Map<List<StockHistoryDto>>(result.Data);
            return Ok(historyDtos);
        }

        // GET: api/stocks/check-level?articleId={articleId}&quantiteDemandee={quantiteDemandee}
        [HttpGet("check-level")]
        public async Task<ActionResult> CheckStockLevel([FromQuery] int articleId, [FromQuery] int quantiteDemandee)
        {
            var result = await _stockService.CheckStockLevel(articleId, quantiteDemandee);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        // POST: api/stocks/reapprovisionner
        [HttpPost("reapprovisionner")]
        public async Task<ActionResult> Reapprovisionner()
        {
            var result = await _stockService.CheckAndReapprovisionner();
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }
    }
}