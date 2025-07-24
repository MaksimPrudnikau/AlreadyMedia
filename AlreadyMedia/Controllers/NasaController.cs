using AlreadyMedia.Contexts;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlreadyMedia.Controllers;

[ApiController]
[Route("[controller]")]
public class NasaController (AppDbContext dbContext)
{
    [HttpGet("dataset")]
    public async Task<ActionResult<IEnumerable<NasaDataset>>> GetDataset()
    {
        return await dbContext.NasaDbSet.AsNoTracking().ToListAsync();
    }
}