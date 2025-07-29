using Core.Extensions;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public interface INasaService
{
    Task<NasaDatasetListResponse> GetFilteredDatasetListResponse(NasaDatasetListRequest request);
}

public class NasaService(AppDbContext dbContext, INasaCacheService cacheService) : INasaService
{
    public async Task<NasaDatasetListResponse> GetFilteredDatasetListResponse(NasaDatasetListRequest request)
    {
        var cached = cacheService.Get(request);
        if (cached is not null)
        {
            return cached;
        }

        var query = BuildQueryFromFilters(request)
            .GroupBy(x => x.Year);

        double totalGroupsCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalGroupsCount / request.ItemsPerPage);

        var groupedDataset = await query
            .Select(x => new NasaDatasetGroupedModel
            {
                Year = x.Key.HasValue ? x.Key.Value.Year : null,
                Count = x.Count(),
                Mass = x.Average(item => item.Mass ?? 0)
            })
            .OrderBy(x => x.Year)
            .ApplyPagination(request)
            .ToListAsync();

        var str = query.ToQueryString();

        var recClasses = await dbContext.NasaDbSet
            .Where(x => x.RecClass != null)
            .GroupBy(x => x.RecClass)
            .Select(x => x.Key!)
            .ToListAsync();

        var res = new NasaDatasetListResponse
        {
            Pagination = new() { TotalPages = totalPages },
            Dataset = groupedDataset,
            RecClasses = recClasses
        };

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
                .Where(x => x.RecClass!.Equals(request.RecClass));
        }

        if (request.NameContains is not null)
        {
            query = query
                .Where(x => x.Name != null)
                .Where(x => x.Name!.Contains(request.NameContains));
        }

        return query;
    }
}