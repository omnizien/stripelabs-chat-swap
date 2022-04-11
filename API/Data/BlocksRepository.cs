using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public class BlocksRepository : IBlocksRepository
    {
        private readonly DataContext _context;

        public BlocksRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<UserBlock> GetUserBlock(int sourceUserId, int blockedUserId)
        {
           return await _context.Blocks.FindAsync(sourceUserId, blockedUserId);
        }

        public async Task<IEnumerable<BlockDto>> GetUserBlocks(string predicate, int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var blocks = _context.Blocks.AsQueryable();


            if (predicate == "blocked")
            {
                blocks = blocks.Where(block => block.SourceUserId == userId); 
                users = blocks.Select(block => block.BlockedUser);
            }

             if (predicate == "blockedBy")
            {
                blocks = blocks.Where(block => block.BlockedUserId == userId);
                users = blocks.Select(block => block.SourceUser);
            }

 

       

            return await users.Select(user => new BlockDto 
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id

            }).ToListAsync();
        }

        public async Task<AppUser> GetUserWithBlocks(int userId)
        {
            return await _context.Users
            .Include(x => x.BlockedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId);        
        }
    }
}