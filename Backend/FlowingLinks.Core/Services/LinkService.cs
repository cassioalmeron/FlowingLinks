using FlowingLinks.Core.Dtos;
using FlowingLinks.Core.Extensions;
using FlowingLinks.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowingLinks.Core.Services
{
    public class LinkService
    {
        private readonly FlowingLinksDbContext _dbContext;

        public LinkService(FlowingLinksDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Link>> GetAll(int userId)
        {
            return await _dbContext.Set<Link>()
                .Include(l => l.User)
                .Include(l => l.LinkLabels)
                    .ThenInclude(ll => ll.Label)
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task<Link?> GetById(int id, int userId)
        {
            return await _dbContext.Set<Link>()
                .Include(l => l.User)
                .Include(l => l.LinkLabels)
                    .ThenInclude(ll => ll.Label)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
        }

        public async Task<IEnumerable<Link>> GetByUserId(int userId)
        {
            return await _dbContext.Set<Link>()
                .Include(l => l.User)
                .Include(l => l.LinkLabels)
                    .ThenInclude(ll => ll.Label)
                .Where(l => l.UserId == userId)
                .ToListAsync();
        }

        public async Task Save(LinkDto linkDto, int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                throw new FlowingLinksException($"User with ID {userId} not found.");

            if (linkDto.Id == 0)
            {
                // Creating new link
                var link = linkDto.CopyTo<Link>();
                link.UserId = userId;
                _dbContext.Set<Link>().Add(link);
                await _dbContext.SaveChangesAsync();
                linkDto.Id = link.Id;
                
                // Add label relationships
                await AddLinkLabels(link.Id, linkDto.LabelIds);
            }
            else
            {
                // Updating existing link
                var link = await _dbContext.Set<Link>().FindAsync(linkDto.Id);
                if (link == null)
                    throw new FlowingLinksException($"Link with ID {linkDto.Id} not found.");
                
                linkDto.CopyTo(link);
                await _dbContext.SaveChangesAsync();
                
                // Update label relationships
                await UpdateLinkLabels(link.Id, linkDto.LabelIds);
            }
        }

        public async Task<bool> Delete(int id, int userId)
        {
            var link = await _dbContext.Set<Link>()
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);
            if (link == null)
                return false;
            
            // Remove all label relationships first
            var linkLabels = await _dbContext.Set<LinkLabel>()
                .Where(ll => ll.LinkId == id)
                .ToListAsync();
            _dbContext.Set<LinkLabel>().RemoveRange(linkLabels);
            
            _dbContext.Set<Link>().Remove(link);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LinkExists(int id, int userId)
        {
            return await _dbContext.Set<Link>().AnyAsync(l => l.Id == id && l.UserId == userId);
        }

        private async Task AddLinkLabels(int linkId, List<int> labelIds)
        {
            if (labelIds == null || !labelIds.Any())
                return;

            var linkLabels = labelIds.Select(labelId => new LinkLabel
            {
                LinkId = linkId,
                LabelId = labelId
            });

            _dbContext.Set<LinkLabel>().AddRange(linkLabels);
            await _dbContext.SaveChangesAsync();
        }

        private async Task UpdateLinkLabels(int linkId, List<int> newLabelIds)
        {
            // Remove existing relationships
            var existingLinkLabels = await _dbContext.Set<LinkLabel>()
                .Where(ll => ll.LinkId == linkId)
                .ToListAsync();
            
            _dbContext.Set<LinkLabel>().RemoveRange(existingLinkLabels);
            
            // Add new relationships
            if (newLabelIds != null && newLabelIds.Any())
            {
                var newLinkLabels = newLabelIds.Select(labelId => new LinkLabel
                {
                    LinkId = linkId,
                    LabelId = labelId
                });
                
                _dbContext.Set<LinkLabel>().AddRange(newLinkLabels);
            }
            
            await _dbContext.SaveChangesAsync();
        }
    }
} 