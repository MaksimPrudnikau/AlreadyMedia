using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        var query = BildQueryFromFilters(fromYear, toYear, recclass)
            .Skip(page * itemsPerPage)
            .Take(itemsPerPage)
            .GroupBy(x => x.Year);

        double itemsCount = await query.CountAsync();
        var pages = (int)Math.Ceiling(itemsCount / itemsPerPage);
        
        var res = await query
            .Select(x => new
            {
                Year = x.Key, 
                Count = x.Count(), 
                Mass = x.Average(item => item.Mass ?? 0)
            })
            .OrderBy(x => x.Year)
            .ToListAsync();


        return Ok(new { pages, data = res });

    }

    private IQueryable<NasaDataset> BildQueryFromFilters(
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