using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public interface INasaService
{
    Task<ICollection<NasaDatasetListResponse>> GetFilteredDatasetListResponse(NasaDatasetListRequest request);
}

public class NasaService(AppDbContext dbContext, NasaCacheService cacheService): INasaService
{
    public async Task<ICollection<NasaDatasetListResponse>> GetFilteredDatasetListResponse(NasaDatasetListRequest request)
    {
        var cached = cacheService.Get(request);
        if (cached is not null)
        {
            return cached;
        }
            
        var query = BuildQueryFromFilters(request)
            .Skip(request.Page * request.ItemsPerPage)
            .Take(request.ItemsPerPage)
            .GroupBy(x => x.Year);

        double itemsCount = await query.CountAsync();
        var pages = (int)Math.Ceiling(itemsCount / request.ItemsPerPage);
        
        var res = await query
            .Select(x => new NasaDatasetListResponse
            {
                Year = x.Key, 
                Count = x.Count(), 
                Mass = x.Average(item => item.Mass ?? 0)
            })
            .OrderBy(x => x.Year)
            .ToListAsync();
        
        cacheService.Save(request, res);

        return res;
    }
    
    private IQueryable<NasaDataset> BuildQueryFromFilters(NasaDatasetListRequest request)
    {
        var query = dbContext.NasaDbSet
            .AsNoTracking();

        if (request.FromYear.HasValue)
        {
            query = query
                .Where(x => x.Year.HasValue)
                .Where(x => x.Year!.Value.Year >= request.FromYear.Value);
        }

        if (request.ToYear.HasValue)
        {
            query = query
                .Where(x => !x.Year.HasValue || x.Year!.Value.Year <= request.ToYear.Value);
        }

        if (request.RecClass is not null)
        {
            query = query
                .Where(x => x.RecClass != null)
                .Where(x => x.RecClass!.Equals(request.RecClass, StringComparison.InvariantCulture));
        }


        return query;
    }
}