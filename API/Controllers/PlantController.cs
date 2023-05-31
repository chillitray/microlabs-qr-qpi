using System.Security.Claims;
using API.DTOs;
using API.Middleware;
using Application.Core;
using Application.DTOs;
using Application.Plants;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.Controllers
{
    [CustomAuthorization(AccessLevelsDto.ADMIN)]
    public class PlantController :BaseApiController
    {
        private readonly IMediator _meditor;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public PlantController(IMediator mediator, UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            this._meditor = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetPlants(Guid id, [FromQuery]PagingParams param) //[FromQuery]PagingParams param
        {
            //  this api fetches all the plants that are linked to the specific product
            // {id} is the product_id
            var result = await Mediator.Send(new List.Query{Id = id, Params=param});
            if (result==null) return NotFound();
            return Ok(PagedResult<List<FetchPlantsDto>>.Success(result.Value,result.PageNumber,result.PageSize,result.TotalRecords));
        }
        

        [HttpPost("{id}")]
        public async Task<IActionResult> CreatePlant(PlantDto plant, Guid id){
            //  this api create the plant and link to the specific product
            // {id} is the product_id
            var logged_user =await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return HandleResult(await Mediator.Send(new Create.Command{plant = plant, product_id = id,logged_user = logged_user}));
        }

        [HttpPost("delete/{plant_id}")]
        public async Task<IActionResult> DeletePlant(Guid plant_id){
            //  this api deactivates/activates the entire plant
            var logged_user =await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return HandleResult(await Mediator.Send(new Delete.Command{PlantId = plant_id,logged_user = logged_user}));
        }

        [HttpPost("unlink/{id}/{plant_id}")]
        public async Task<IActionResult> UnlinkProduct(Guid id, Guid plant_id){
            //  this api unlinks / links the product-plant mapping 
            // {id} is the product_id
            var logged_user =await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            return HandleResult(await Mediator.Send(new UnlinkProduct.Command{ProductId=id,PlantId=plant_id,logged_user = logged_user}));
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditPlant(Plant plant){
            //  this api edit the plant details
            var logged_user =await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return HandleResult(await Mediator.Send(new EditPlant.Command{plant = plant,logged_user = logged_user}));
        }

        [HttpPost("link/{product_id}/{plant_id}")]
        public async Task<IActionResult> LinkProduct(Guid product_id, Guid plant_id ){
            //  this api creates the link between product and plant
            var logged_user =await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return HandleResult(await Mediator.Send(new LinkProduct.Command{ProductId=product_id,PlantId=plant_id,logged_user = logged_user}));
        }

        [HttpGet("details/{plant_id}")]
        public async Task<IActionResult> PlantDetails(Guid plant_id ){
            //  this api retund the details of the plant along with the products that are linked to it

            return HandleResult(await Mediator.Send(new FetchPlantDetails.Query{PlantId=plant_id}));
        }
    }
}