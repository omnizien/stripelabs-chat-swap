using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Data;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{

    [Authorize]
    public class BlocksController : BaseApiController
    {
        public IUnitOfWork _unitOfWork;
        private readonly DataContext _context;

        public BlocksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;


        }


        [HttpPost("{username}")]
        public async Task<ActionResult> AddBlock(string username)
        {
            var sourceUserId = User.GetUserId();
            var blockedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _unitOfWork.BlocksRepository.GetUserWithBlocks(sourceUserId);
            if (blockedUser == null) return NotFound();
            if (sourceUser.UserName == username) return BadRequest("You Cannot Block Yourself");
            var userBlock = await _unitOfWork.BlocksRepository.GetUserBlock(sourceUserId, blockedUser.Id);
            if (userBlock != null) return BadRequest("You already block this user");
            userBlock = new UserBlock
            {
                SourceUserId = sourceUserId,
                BlockedUserId = blockedUser.Id
            };

            sourceUser.BlockedUsers.Add(userBlock);
            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest();
        }


        [HttpDelete("{username}")]
        public async Task<ActionResult> RemoveBlock(string username)
        {
            var blockedUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            var sourceUserId = User.GetUserId();
            var sourceUser = await _unitOfWork.BlocksRepository.GetUserWithBlocks(sourceUserId);
            var userBlock = await _unitOfWork.BlocksRepository.GetUserBlock(sourceUserId, blockedUser.Id);
            // sourceUser.BlockedUsers.Remove(userBlock);
            // if (await _unitOfWork.Complete()) return Ok();

            if (userBlock != null)
            {
                sourceUser.BlockedUsers.Remove(userBlock);
                if (await _unitOfWork.Complete()) return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlockDto>>> GetUserBlocks(string predicate)
        {
            // blocksParams.UserId = User.GetUserId();
            // var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

            // Response.AddPaginationHeader(users.CurrentPage,
            //     users.PageSize, users.TotalCount, users.TotalPages);

            // return Ok(users);

            var users = await _unitOfWork.BlocksRepository.GetUserBlocks(predicate, User.GetUserId());
            return Ok(users);
        }

    }

}


// DELETE: api/TodoItems/5
// [HttpDelete("{id}")]
// public async Task<ActionResult<ToDoItem>> DeleteTodoItem(long id)
// {
//     var todoItem = await _context.TodoItems.FindAsync(id);
//     if (todoItem == null)
//     {
//         return NotFound();
//     }

//     _context.TodoItems.Remove(todoItem);
//     await _context.SaveChangesAsync();

//     return todoItem;
// }