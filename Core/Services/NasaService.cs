using Core.Extensions;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Services;

public interface INasaService
{
    Task<NasaDatasetListResponse> GetFilteredDatasetListResponse(NasaDatasetListRequest request, CancellationToken token = default);
}

public class NasaService(AppDbContext dbContext, INasaCacheService cacheService) : INasaService
{
    public async Task<NasaDatasetListResponse> GetFilteredDatasetListResponse(NasaDatasetListRequest request, CancellationToken token = default)
    {
        var cached = await cacheService.GetAsync(request, token);
        if (cached is not null)
        {
            return cached;
        }

        var query = BuildQueryFromFilters(request)
            .GroupBy(x => x.Date);

        double totalGroupsCount = await query.CountAsync(cancellationToken: token);
        var totalPages = (int)Math.Ceiling(totalGroupsCount / request.ItemsPerPage);

        var groupedDataset = await GetGroupedDatasetAsync(query, request, token);

        var recClasses = await dbContext.NasaDbSet
            .Where(x => x.RecClass != null)
            .GroupBy(x => x.RecClass)
            .Select(x => x.Key!)
            .ToListAsync(cancellationToken: token);

        var res = new NasaDatasetListResponse
        {
            Pagination = new() { TotalPages = totalPages },
            Dataset = groupedDataset,
            RecClasses = recClasses
        };

        await cacheService.SaveAsync(request, res);

        return res;
    }

    private IQueryable<NasaDataset> BuildQueryFromFilters(NasaDatasetListRequest request)
    {
        var query = dbContext.NasaDbSet
            .AsNoTracking();

        if (request.FromYear.HasValue)
        {
            query = query
                .Where(x => x.Date.HasValue)
                .Where(x => x.Date!.Value.Year >= request.FromYear.Value);
        }

        if (request.ToYear.HasValue)
        {
            var startDate = new DateTime(1, 1, request.ToYear.Value);
            var endDate = new DateTime(1, 1, request.ToYear.Value + 1).AddMilliseconds(-1);
            
            query = query
                .Where(x => x.Date >= startDate);
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
        CancellationToken token = default)
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
            .ToListAsync(token);
    }
}