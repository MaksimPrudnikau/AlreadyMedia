using AlreadyMedia.Contexts;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AlreadyMedia.Controllers;

[ApiController]
[Route("[controller]")]
public class NasaController (AppDbContext dbContext): ControllerBase
{
    [HttpGet("dataset")]
    public async Task<IActionResult> GetDataset(
        [FromQuery]int? fromYear, 
        [FromQuery]int? toYear,
        [FromQuery]string? recclass, 
        [FromQuery]int page = 0, 
        [FromQuery]int itemsPerPage = 10)
    {

        var query = BuildQuery(fromYear, toYear, recclass);
        
        var res = query
            .Skip(page * itemsPerPage)
            .Take(itemsPerPage)
            .GroupBy(x => x.Year)
            .OrderBy(x => x.Key)
            .ToListAsync();
        
        var pages = await dbContext.NasaDbSet.CountAsync();

        return Ok(res);

    }

    private IQueryable<NasaDataset> BuildQuery(
        int? fromYear,
        int? toYear,
        string? recClass)
    {
        var query = dbContext.NasaDbSet
            .AsNoTracking();

        if (fromYear.HasValue)
        {
            query = query
                .Where(x => x.Year.HasValue)
                .Where(x => x.Year!.Value.Year >= fromYear.Value);
        }

        if (toYear.HasValue)
        {
            query = query
                .Where(x => !x.Year.HasValue || x.Year!.Value.Year <= toYear.Value);
        }

        if (recClass is not null)
        {
            query = query
                .Where(x => x.RecClass != null)
                .Where(x => x.RecClass!.Equals(recClass, StringComparison.InvariantCulture));
        }


        return query;
    }
}