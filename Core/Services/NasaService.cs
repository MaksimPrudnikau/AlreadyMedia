using Core.Extensions;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public interface INasaService
{
    Task<NasaDatasetListResponse> GetFilteredDatasetListResponse(NasaDatasetListRequest request, CancellationToken ct = default);
}

public class NasaService(AppDbContext dbContext, INasaCacheService cacheService) : INasaService
{
    public async Task<NasaDatasetListResponse> GetFilteredDatasetListResponse(NasaDatasetListRequest request, CancellationToken ct = default)
    {
        var cached = await cacheService.GetAsync(request, ct);
        if (cached is not null)
        {
            return cached;
        }

        var query = BuildQueryFromFilters(request)
            .GroupBy(x => x.Date);

        double totalGroupsCount = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalGroupsCount / request.ItemsPerPage);

        var groupedDataset = await GetGroupedDatasetAsync(query, request, ct);

        var recClasses = await dbContext.NasaDbSet
            .Where(x => x.RecClass != null)
            .GroupBy(x => x.RecClass)
            .Select(x => x.Key!)
            .ToListAsync(ct);

        var res = new NasaDatasetListResponse
        {
            Pagination = new() { TotalPages = totalPages },
            Dataset = groupedDataset,
            RecClasses = recClasses
        };

        await cacheService.SaveAsync(request, res, ct);

        return res;
    }

    private IQueryable<NasaDataset> BuildQueryFromFilters(NasaDatasetListRequest request)
    {
        var query = dbContext.NasaDbSet
            .AsNoTracking();

        if (request.FromYear.HasValue)
        {
            var startDate = new DateTime(request.FromYear.Value, 1, 1);
            
            query = query
                .Where(x => x.Date >= startDate);
        }

        if (request.ToYear.HasValue)
        {
            var endDate = new DateTime(request.ToYear.Value + 1, 1, 1);
            
            query = query
                .Where(x => x.Date <= endDate);
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

    private async static Task<IEnumerable<NasaDatasetGroupedModel>> GetGroupedDatasetAsync(
        IQueryable<IGrouping<DateTime?, NasaDataset>> query,
        NasaDatasetListRequest request,
        CancellationToken ct = default)
    {
        return await query
            .Select(x => new NasaDatasetGroupedModel
            {
                Year = x.Key.HasValue ? x.Key.Value.Year : null,
                Count = x.Count(),
                Mass = x.Average(item => item.Mass ?? 0)
            })
            .OrderBy(x => x.Year)
            .ApplyPagination(request)
            .ToListAsync(ct);
    }
}