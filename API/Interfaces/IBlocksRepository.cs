using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IBlocksRepository
    {


          Task<UserBlock> GetUserBlock(int sourceUserId, int blockedUserId);
           Task<AppUser> GetUserWithBlocks(int userId);
           Task<IEnumerable<BlockDto>> GetUserBlocks(string predicate, int userId);
        
    }
}