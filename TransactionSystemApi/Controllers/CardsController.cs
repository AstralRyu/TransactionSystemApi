using Microsoft.AspNetCore.Mvc;
using TransactionSystemApi.DTOs;
using TransactionSystemApi.Models;
using TransactionSystemApi.Services;

namespace TransactionSystemApi.Controllers;

[ApiController]
[Route("card")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardsController(ICardService cardService)
    {
        _cardService = cardService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<Card>> CreateCard([FromBody] CreateCardRequest request)
    {
        if (request.CreditLimit <= 0)
            return BadRequest("Credit limit must be greater than zero.");

        try
        {
            var card = await _cardService.CreateCardAsync(request);
            return Ok(card);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }
    
    [HttpGet("{cardId:guid}/balance")]
    public async Task<ActionResult<decimal>> GetCardBalance(Guid cardId, [FromQuery] string currency)
    {
        var balance = await _cardService.GetCardBalanceAsync(cardId, currency);
        return Ok(balance);
    }
}