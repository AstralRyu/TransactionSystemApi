using Microsoft.AspNetCore.Mvc;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Controllers;

[ApiController]
[Route("cards")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    // POST /cards
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardRequest request)
    {
        if (request.CreditLimit <= 0)
            return BadRequest("Credit limit must be greater than zero.");

        try
        {
            var card = await _cardService.CreateCardAsync(request.CreditLimit);
            return CreatedAtAction(nameof(GetBalance), new { cardId = card.Id }, card);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    // GET /cards/{cardId}/balance?currency=CAD
    [HttpGet("{cardId:guid}/balance")]
    public async Task<IActionResult> GetBalance(Guid cardId, [FromQuery] string currency = "USD")
    {
        var balance = await _cardService.GetCardBalanceAsync(currency);;
        return Ok(balance);
    }
}