using Microsoft.AspNetCore.Mvc;
using PharmacyApi.Models;
using PharmacyApi.Services;

namespace PharmacyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly PharmacyService _service = new();

    [HttpGet]
    public IActionResult GetAll() => Ok(_service.GetAllMedicines());

    [HttpGet("stocks")]
    public IActionResult GetStocks([FromQuery] int? medicineId, [FromQuery] string? warehouse)
        => Ok(_service.GetStocks(medicineId, warehouse));

    [HttpGet("report")]
    public IActionResult GetReport() => Ok(_service.GetOrderReport());

    [HttpPost("receive")]
    public IActionResult Receive([FromBody] ReceiveDto dto)
    {
        _service.Receive(dto.MedicineId, dto.Warehouse, dto.Quantity, dto.ExpiryDate);
        return Ok(new { message = "Поступление сохранено" });
    }

    [HttpPost("writeoff")]
    public IActionResult WriteOff([FromBody] WriteOffDto dto)
    {
        var ok = _service.WriteOff(dto.MedicineId, dto.Warehouse, dto.Quantity, dto.Reason);
        return ok ? Ok(new { message = "Списано" }) : BadRequest(new { message = "Недостаточно остатка" });
    }

    [HttpPost("move")]
    public IActionResult Move([FromBody] MoveDto dto)
    {
        var ok = _service.Move(dto.MedicineId, dto.FromWarehouse, dto.ToWarehouse, dto.Quantity);
        return ok ? Ok(new { message = "Перемещено" }) : BadRequest(new { message = "Ошибка перемещения" });
    }
}

public class ReceiveDto
{
    public int MedicineId { get; set; }
    public string Warehouse { get; set; } = "";
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; set; } = DateTime.Today.AddYears(2);
}

public class WriteOffDto
{
    public int MedicineId { get; set; }
    public string Warehouse { get; set; } = "";
    public int Quantity { get; set; }
    public string Reason { get; set; } = "";
}

public class MoveDto
{
    public int MedicineId { get; set; }
    public string FromWarehouse { get; set; } = "";
    public string ToWarehouse { get; set; } = "";
    public int Quantity { get; set; }
}