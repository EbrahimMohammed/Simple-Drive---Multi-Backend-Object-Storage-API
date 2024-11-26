using Data.Models;
using Data;
using Microsoft.AspNetCore.Http;
using Business.BlobTrackingsRepository;
using Microsoft.EntityFrameworkCore;
using Business.UserContextService;

public class BlobTrackingsRepository : IBlobTrackingsRepository
{
    private readonly AppDbContext _context;
    private readonly IUserContextService _userContextService;

    public BlobTrackingsRepository(AppDbContext context, IUserContextService userContextService)
    {
        _context = context;
        _userContextService = userContextService;
    }

    public async Task Add(BlobTracking blobTracking)
    {
        await _context.BlobTrackings.AddAsync(blobTracking);
        await _context.SaveChangesAsync();
    }

    public async Task<BlobTracking> GetById(string id)
    {
       
        return await _context.BlobTrackings
            .FirstOrDefaultAsync(x => x.Id == id && x.CreatedBy == _userContextService.UserId);
    }

    public async Task<bool> BlobIdExists(string id)
    {
        return await _context.BlobTrackings.AnyAsync(x => x.Id == id);
    }
}
