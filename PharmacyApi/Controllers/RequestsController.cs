using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Services;

namespace PharmacyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly PharmacyService _service = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAllRequests());

    [HttpPost("issue/{itemId}")]
    public IActionResult Issue(int itemId)
    {
        _service.IssueRequestItem(itemId);
        return Ok(new { message = "Отпущено" });
    }
}