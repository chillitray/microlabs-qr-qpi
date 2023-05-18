using API.DTOs;
using API.Middleware;
using Application.Admins;
using Application.Core;
using Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers
{
    
    public class AdminController :BaseApiController 
    {
        private readonly IMediator _meditor;
        private readonly DataContext _context;
        public AdminController(IMediator mediator, DataContext context)
        {
            this._context = context;
            this._meditor = mediator;
        }

        [CustomAuthorization(AccessLevelsDto.ADMIN)]
        [HttpGet]
        public async Task<IActionResult>GetAdmins([FromQuery]PagingParams param) //[FromQuery]PagingParams param
        {
            //  this api fetches all the admins
            var result = await Mediator.Send(new ListAdmins.Query{Params=param});
            if (result==null) return NotFound();
            return Ok(PagedResult<List<FetchAdminsDto>>.Success(result.Value,result.PageNumber,result.PageSize,result.TotalRecords));
        }
    }
}