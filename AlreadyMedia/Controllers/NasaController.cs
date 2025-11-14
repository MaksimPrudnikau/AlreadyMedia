using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace AlreadyMedia.Controllers;

[ApiController]
[Route("[controller]")]
public class NasaController (INasaService nasaService): ControllerBase
{
    
    [HttpGet("dataset")]
    [ProducesResponseType<NasaDatasetListResponse>(StatusCodes.Status200OK)]
    public Task<NasaDatasetListResponse> GetDataset([FromQuery] NasaDatasetListRequest request, CancellationToken ct = default)
    {
        return nasaService.GetFilteredDatasetListResponse(request, ct);
    }
}