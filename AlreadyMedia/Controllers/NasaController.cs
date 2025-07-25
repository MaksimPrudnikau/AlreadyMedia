using Core;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AlreadyMedia.Controllers;

[ApiController]
[Route("[controller]")]
public class NasaController (INasaService nasaService): ControllerBase
{
    
    [HttpGet("dataset")]
    public async Task<IActionResult> GetDataset([FromQuery] NasaDatasetListRequest request)
    {
        return Ok(await nasaService.GetFilteredDatasetListResponse(request));
    }
}