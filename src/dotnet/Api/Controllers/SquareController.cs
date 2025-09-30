using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Api.Controllers;

[Route("calc/[controller]")]
[ApiController]
public class SquareController : ControllerBase
{
    private readonly ILogger _logger;
    
    public SquareController(ILogger<SquareController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("{number}")]
    [Authorize("calc:square")]
    public JsonResult Get(int number)
    {
        _logger.LogInformation("Get square of {number}", number);
        
        return new JsonResult(new ResultModel { Result = number * number });
    }
}