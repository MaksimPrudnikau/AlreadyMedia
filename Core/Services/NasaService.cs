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

        double totalItemsCount = await BuildQueryFromFilters(request).CountAsync();
        var totalPages = (int)Math.Ceiling(totalItemsCount / request.ItemsPerPage);

        var query = BuildQueryFromFilters(request)
            .ApplyPagination(request)
            .GroupBy(x => x.Year);

        var groupedDataset = await query
            .Select(x => new NasaDatasetGroupedModel
            {
                Year = x.Key,
                Count = x.Count(),
                Mass = x.Average(item => item.Mass ?? 0)
            })
            .OrderBy(x => x.Year)
            .ToListAsync();

        var res = new NasaDatasetListResponse
        {
            Pagination = new() { TotalPages = totalPages },
            Dataset = groupedDataset
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
                .Where(x => x.RecClass!.Equals(request.RecClass, StringComparison.InvariantCulture));
        }

        return query;
    }
}