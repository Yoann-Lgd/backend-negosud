using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using backend_negosud.DTOs;
using backend_negosud.Services;
using backend_negosud.Entities;
using backend_negosud.Repository;
using backend_negosud.Validation;

namespace backend_negosud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService _stockService;
        private readonly IMapper _mapper;

        public StocksController(IStockService stockService, IMapper mapper)
        {
            _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        // GET: api/stocks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockSummaryDto>>> GetAllStocks()
        {
            var stocks = await _stockService.GetAllStocks();
            return Ok(stocks);
        }

        // GET: api/stocks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StockDetailDto>> GetStockById(int id)
        {
            var stock = await _stockService.GetById(id);
            if (stock.Data == null)
            {
                return NotFound();
            }

            var stockDto = _mapper.Map<StockDetailDto>(stock.Data);
            return Ok(stockDto);
        }

        // POST: api/stocks
        [HttpPost]
        public async Task<ActionResult<StockSummaryDto>> CreateStock([FromBody] StockInputDto createStockDto)
        {
            var validation = new StockValidation();
            var validationResult = validation.Validate(createStockDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
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

            Console.WriteLine(result.Data.StockId);
            var stock = await _stockService.GetById(result.Data.StockId);
            var stockDto = _mapper.Map<StockSummaryDto>(stock.Data);
            return CreatedAtAction(nameof(GetStockById), new { id = stock.Data.StockId }, stockDto);
        }

        // PUT: api/stocks/{id}
        /// <summary>
        /// Attention, permet seulement l'update de la quantité, le reste ne sera pas affecté
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("UpdateStockQuantity/{id}")]
        public async Task<IActionResult> UpdateStockQuntity(int id, [FromBody] StockUpdateDto updateStockDto)
        {
            var validation = new StockValidationUpdate();
            var validationResult = validation.Validate(updateStockDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var stock = await _stockService.GetById(id);
            if (stock.Data == null)
            {
                return NotFound();
            }

            var result = await _stockService.UpdateStockQuantity(
                id,
                updateStockDto.nouvelleQuantite,
                updateStockDto.utilisateurId, 
                updateStockDto.typeModification
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
            var stock = await _stockService.GetById(id);
            if (stock == null)
            {
                return NotFound();
            }

            await _stockService.Delete(stock.Data);
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
        public async Task<ActionResult> CheckStockLevel([FromQuery] int articleId)
        {
            var result = await _stockService.CheckStockLevel(articleId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<StockUpdateDto>> UpdateStock(int id, [FromBody] StockInputPatchDto updateStockDto)
        {
            var result = await _stockService.PatchStock(id, updateStockDto);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        /*// POST: api/stocks/reapprovisionner
        [HttpPost("reapprovisionner")]
        public async Task<ActionResult> Reapprovisionner()
        {
            var result = await _stockService.CheckAndReapprovisionner();
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }*/
    }
}