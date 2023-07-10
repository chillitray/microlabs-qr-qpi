
using System.Security.Claims;
using API.DTOs;
using API.DTOs.Product;
using API.Middleware;
using API.Trackers;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [CustomAuthorization(AccessLevelsDto.ADMIN)]
    [ApiController]
    [Route("api/product/")]
    
    public class ProductController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public ProductController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }


        [HttpPost("create/")]
        public async Task<ActionResult> Create(CreateProductDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            // TODO : need to product logo

            //create the record in the db
            var newRecord = _context.ProductManagement.Add(
                new ProductManagement{
                    product_name = managementDto.product_name,
                    product_description = managementDto.product_description,
                    product_writeup = managementDto.product_writeup,
                    product_expiry_days = managementDto.product_expiry_days,
                    product_mrp = managementDto.product_mrp,
                    created_by = logged_user.user_id,
                    product_logo = ""
                }
            );

            // Console.WriteLine("Step0");
            var new_obj_string = new TrackerUtils().CreateProductObj(newRecord.Entity);
            // Console.WriteLine("Step1");
            //create a tracker record
            _context.TrackingProductActivity.Add(
                new TrackingProductActivity{
                    new_obj = new_obj_string,
                    product_id = newRecord.Entity.product_id,
                    user_id = logged_user.user_id
                }
            );
            // Console.WriteLine("Step2");

            // create a record in TrackerActivity
            var activity = _context.TrackingActivity.Add(
                new TrackingActivity{
                    custom_obj = new_obj_string,
                    message = "Added new product",
                    severity_type = SeverityType.CRITICAL,
                    user_id = logged_user.user_id
                }
            );
            // Console.WriteLine("Step3");

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to create the record");
            // Console.WriteLine("Step4");
            return Ok("Record created successfully");

        }


        [HttpPost("edit/")]
        public async Task<ActionResult> Edit(CreateProductDto managementDto)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            //find the record
            var product = _context.ProductManagement.Find(managementDto.product_id);
            if(product==null){
                return NotFound("invalid product_id");
            }
            var tracker_product = product;
            tracker_product.product_logo = product.product_logo ?? "";
            tracker_product.product_writeup = product.product_writeup ?? "";
            var old_obj_string = new TrackerUtils().CreateProductObj(product);

            product.product_name = managementDto.product_name ?? product.product_name;
            product.product_description = managementDto.product_description ?? product.product_description;
            // TODO : need to product logo
            product.product_writeup = managementDto.product_writeup ?? product.product_writeup;
            if(managementDto.product_expiry_days!=0){
                product.product_expiry_days = managementDto.product_expiry_days;
            }
            if(managementDto.product_mrp!=0){
                product.product_mrp = managementDto.product_mrp;
            }
            if(managementDto.status !=null){
                product.status = managementDto.status == "ACTIVE" ? PlantStatusOptions.ACTIVE : PlantStatusOptions.INACTIVE;
            }

            product.last_updated_at = DateTime.Now;
            tracker_product = product;
            tracker_product.product_logo = product.product_logo ?? "";
            tracker_product.product_writeup = product.product_writeup ?? "";

            var new_obj_string = new TrackerUtils().CreateProductObj(tracker_product);
            //create a tracker record
            _context.TrackingProductActivity.Add(
                new TrackingProductActivity{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    product_id = product.product_id,
                    user_id = logged_user.user_id
                }
            );

            // create a record in TrackerActivity
            var activity = _context.TrackingActivity.Add(
                new TrackingActivity{
                    custom_obj = new_obj_string,
                    message = "Edited product",
                    severity_type = SeverityType.CRITICAL,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to Edit the record");

            return Ok("Record edited successfully");

        }



        [HttpPost("media/delete/{id}")]
        public async Task<ActionResult> DeleteMedia(Guid id)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            //find the record
            var product = _context.ProductMedia.Find(id);
            if(product==null){
                return NotFound("invalid product_media_id");
            }
            var old_obj_string = new TrackerUtils().CreateProductMediaObj(product);

            product.status = PlantStatusOptions.INACTIVE;
            product.last_updated_at = DateTime.Now;

            var new_obj_string = new TrackerUtils().CreateProductMediaObj(product);
            //create a tracker record
            _context.TrackingProductMediaActivity.Add(
                new TrackingProductMediaActivity{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    product_media_id = product.product_media_id,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to Edit the record");

            return Ok("Record edited successfully");

        }


        [HttpPost("delete/{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            //find the record
            var product = _context.ProductManagement.Find(id);
            if(product==null){
                return NotFound("invalid product_id");
            }
            var old_obj_string = new TrackerUtils().CreateProductObj(product);

            if(product.status == PlantStatusOptions.ACTIVE){
                product.status = PlantStatusOptions.INACTIVE;
            }else{
                product.status = PlantStatusOptions.ACTIVE;
            }
            
            product.last_updated_at = DateTime.Now;
            var new_obj_string = new TrackerUtils().CreateProductObj(product);
            //create a tracker record
            _context.TrackingProductActivity.Add(
                new TrackingProductActivity{
                    old_obj = old_obj_string,
                    new_obj = new_obj_string,
                    product_id = product.product_id,
                    user_id = logged_user.user_id
                }
            );

            // create a record in TrackerActivity
            var activity = _context.TrackingActivity.Add(
                new TrackingActivity{
                    custom_obj = new_obj_string,
                    message = "Disabled product",
                    severity_type = SeverityType.CRITICAL,
                    user_id = logged_user.user_id
                }
            );

            // #save the changes
            var result = await _context.SaveChangesAsync() >0;
            if(!result) return NotFound("Unable to Edit the record");

            return Ok("Record edited successfully");

        }


        [HttpPost("get/{id}")]
        public async Task<ActionResult<CreateProductDto>> GetProductDetails(Guid id)
        {
            // var logged_user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            //find the record
            var product =await _context.ProductManagement.FindAsync(id);
            if(product==null){
                return NotFound("invalid product_id");
            }
            //fetch the count of plants linked to this product
            var plants_count = _context.ProductPlantMapping.Where(x=>x.product_id==id & x.status==PlantStatusOptions.ACTIVE).ToList().Count;
            //fetch the qr records related to this product
            var qrs = _context.QrManagement.Where(x => x.product_id == id).ToList();

            var no_qrs_generated = qrs.Count;
            //find the qrs with status printed
            var no_qrs_printed = qrs.Where(x => x.status == QrManagementStatus.PRINTED ).ToList().Count;
            //find the qrs counterfieted
            var no_qrs_counterfied = qrs.Where(x=>x.status == QrManagementStatus.COUNTERFEIT_BLOCKED).ToList().Count;
            //find the no_qrs_disabled
            var no_qrs_disabled = qrs.Where(x=>x.status == QrManagementStatus.BLOCKED).ToList().Count;


            //fetch the qr read activity
            var qr_read_records = _context.QrReadActivity.Where(x=>x.product_id==id).ToList();
            var qr_read = 0;
            if(qr_read_records.Count>0){
                qr_read = qr_read_records.GroupBy(x=>x.qr_id).First().ToList().Count;
            }

            var output = new CreateProductDto{
                product_id = product.product_id,
                product_name = product.product_name,
                product_description = product.product_description,
                product_logo = product.product_logo,
                product_writeup = product.product_writeup,
                product_expiry_days = product.product_expiry_days,
                product_mrp = product.product_mrp,
                status = product.status.ToString(),
                created_at = product.created_at,
                last_updated_at = product.last_updated_at,
                no_of_plants = plants_count,
                no_qrs_generated=no_qrs_generated,
                no_qrs_printed=no_qrs_printed,
                no_qrs_unused = no_qrs_generated - no_qrs_printed,
                no_qrs_scanned = qr_read,
                no_qrs_counterfied = no_qrs_counterfied,
                no_qrs_disabled = no_qrs_disabled

            };

            return Ok(output);

        }


        [HttpPost("get/all/")]
        public async Task<PagedResult<List<FetchAllProducts>>> FetchAll([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.ProductManagement.Where(x => true).OrderBy(x => x.product_name).ToListAsync();
            var count = email_x.Count;

            // //sort the records in descending order and fetch the product ids
            // var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            // if(!param.Sort){
            //     email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            // }

            List<FetchAllProducts> details_final = new List<FetchAllProducts>();
            foreach(ProductManagement em in email_x){
                var data=new FetchAllProducts{

                        product_id = em.product_id,
                        product_name = em.product_name,
                        status = em.status.ToString(),
                    };

                

                details_final.Add(data);
            };

            return PagedResult<List<FetchAllProducts>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }



    }
}