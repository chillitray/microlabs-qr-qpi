using API.DTOs;
using API.DTOs.Trackers;
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
    [Route("api/trackers")]
    public class TrackerController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public TrackerController(UserManager<User> userManager, DataContext context)
        {
            this._context = context;
            this._userManager = userManager;
            
        }

        [HttpPost("session/get/all/")]
        public async Task<PagedResult<List<SessionDto>>> FetchSessionTrackers([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.SessionActivity.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.last_login).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.last_login).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> session_ids = new List<Guid?>();            
            foreach(SessionActivity plt in email){                 
                user_ids.Add(plt.user_id);
                session_ids.Add(plt.session_id);                    
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the inactive sessions
            var inactives = _context.InactiveSessionActivity.Where(x=> session_ids.Contains(x.session_id)).ToList();

            List<SessionDto> details_final = new List<SessionDto>();
            foreach(SessionActivity em in email){
                var data=new SessionDto{
                        user_id = em.user_id,
                        last_login = em.last_login,
                        last_login_ip = em.last_login_ip,
                        last_access = em.last_access,
                        last_access_ip = em.last_access_ip,
                        // logout_status = em.status.ToString(),
                        // expired_at = em.expired_at 
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(InactiveSessionActivity ses in inactives){
                    if(em.session_id == ses.session_id){
                        data.logout_status = ses.status.ToString();
                        data.expired_at = ses.expired_at;
                    }
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<SessionDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }

        [HttpPost("session/get/{id}/")]
        public async Task<PagedResult<List<SessionDto>>> FetchUserSessionTrackers(Guid id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.SessionActivity.Where(x => x.user_id==id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.last_login).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.last_login).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> session_ids = new List<Guid?>();            
            foreach(SessionActivity plt in email){                 
                user_ids.Add(plt.user_id);
                session_ids.Add(plt.session_id);                    
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the inactive sessions
            var inactives = _context.InactiveSessionActivity.Where(x=> session_ids.Contains(x.session_id)).ToList();

            List<SessionDto> details_final = new List<SessionDto>();
            foreach(SessionActivity em in email){
                var data=new SessionDto{
                        user_id = em.user_id,
                        last_login = em.last_login,
                        last_login_ip = em.last_login_ip,
                        last_access = em.last_access,
                        last_access_ip = em.last_access_ip,
                        // logout_status = em.status.ToString(),
                        // expired_at = em.expired_at 
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(InactiveSessionActivity ses in inactives){
                    if(em.session_id == ses.session_id){
                        data.logout_status = ses.status.ToString();
                        data.expired_at = ses.expired_at;
                    }
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<SessionDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("plant/get/all/")]
        public async Task<PagedResult<List<PlantDto>>> FetchAllPlantsActivities([FromQuery]PagingParams param)
        {
            // TODO : need to update the logic
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingPlantActivity.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> plant_ids = new List<Guid?>();            
            foreach(TrackingPlantActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                plant_ids.Add(plt.plant_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the plant details
            var plants = _context.Plant.Where(x=> plant_ids.Contains(x.plant_id)).ToList();
            
            var utils = new TrackerUtils();
            List<Guid> user_ids2 = new List<Guid>();
            List<PlantDto> details_final = new List<PlantDto>();
            foreach(TrackingPlantActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }

                    if(changes.ContainsKey("operated_id")){
                        user_ids2.Add(new Guid(changes["operated_id"]["old_value"]));
                        user_ids2.Add(new Guid(changes["operated_id"]["new_value"]));
                    }
                }

                var data=new PlantDto{
                    plant_id = em.plant_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(Plant user in plants){
                    if(em.plant_id == user.plant_id){
                        data.plant_name = user.plant_name;
                        data.plant_code = user.plant_code;
                    }
                }

                

                details_final.Add(data);
            };

            if(user_ids2.Count>0){
                users = _context.User.Where(x=>user_ids2.Contains(x.user_id)).ToList();
                foreach(PlantDto record in details_final){
                    if(record.changes.ContainsKey("operated_id")){
                        foreach(User usr in users){
                            if(usr.user_id==new Guid(record.changes["operated_id"]["old_value"])){
                                record.changes["operated_id"]["old_name"] = usr.full_name;
                            }
                            if(usr.user_id==new Guid(record.changes["operated_id"]["new_value"])){
                                record.changes["operated_id"]["new_name"] = usr.full_name;
                            }
                        
                        }
                    }
                }
            }

            return PagedResult<List<PlantDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("plant/get/{id}/")]
        public async Task<PagedResult<List<PlantDto>>> FetchOnePlantActivities(Guid id,[FromQuery]PagingParams param)
        {
            // TODO : need to update the logic
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingPlantActivity.Where(x => x.plant_id == id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> plant_ids = new List<Guid?>();            
            foreach(TrackingPlantActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                plant_ids.Add(plt.plant_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the plant details
            var plants = _context.Plant.Where(x=> plant_ids.Contains(x.plant_id)).ToList();
            
            var utils = new TrackerUtils();
            List<Guid> user_ids2 = new List<Guid>();
            List<PlantDto> details_final = new List<PlantDto>();
            foreach(TrackingPlantActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                    if(changes.ContainsKey("operated_id")){
                        user_ids2.Add(new Guid(changes["operated_id"]["old_value"]));
                        user_ids2.Add(new Guid(changes["operated_id"]["new_value"]));
                    }
                }

                var data=new PlantDto{
                    plant_id = em.plant_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(Plant user in plants){
                    if(em.plant_id == user.plant_id){
                        data.plant_name = user.plant_name;
                        data.plant_code = user.plant_code;
                    }
                }

                

                details_final.Add(data);
            };
            if(user_ids2.Count>0){
                users = _context.User.Where(x=>user_ids2.Contains(x.user_id)).ToList();
                foreach(PlantDto record in details_final){
                    if(record.changes.ContainsKey("operated_id")){
                        foreach(User usr in users){
                            if(usr.user_id==new Guid(record.changes["operated_id"]["old_value"])){
                                record.changes["operated_id"]["old_name"] = usr.full_name;
                            }
                            if(usr.user_id==new Guid(record.changes["operated_id"]["new_value"])){
                                record.changes["operated_id"]["new_name"] = usr.full_name;
                            }
                        
                        }
                    }
                }
            }

            return PagedResult<List<PlantDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("product/get/all/")]
        public async Task<PagedResult<List<ProductDto>>> FetchAllProductActivities([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingProductActivity.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> product_ids = new List<Guid?>();            
            foreach(TrackingProductActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                product_ids.Add(plt.product_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the product details
            var products = _context.ProductManagement.Where(x=> product_ids.Contains(x.product_id)).ToList();
            
            var utils = new TrackerUtils();

            List<ProductDto> details_final = new List<ProductDto>();
            foreach(TrackingProductActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new ProductDto{
                    product_id = em.product_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(ProductManagement user in products){
                    if(em.product_id == user.product_id){
                        data.product_name = user.product_name;
                    }
                }
                

                details_final.Add(data);
            };

            return PagedResult<List<ProductDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("product/get/{id}/")]
        public async Task<PagedResult<List<ProductDto>>> FetchOneProductActivities(Guid id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingProductActivity.Where(x => x.product_id==id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> product_ids = new List<Guid?>();            
            foreach(TrackingProductActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                product_ids.Add(plt.product_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the product details
            var products = _context.ProductManagement.Where(x=> product_ids.Contains(x.product_id)).ToList();
            
            var utils = new TrackerUtils();

            List<ProductDto> details_final = new List<ProductDto>();
            foreach(TrackingProductActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new ProductDto{
                    product_id = em.product_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(ProductManagement user in products){
                    if(em.product_id == user.product_id){
                        data.product_name = user.product_name;
                    }
                }
                

                details_final.Add(data);
            };

            return PagedResult<List<ProductDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("media/get/all/")]
        public async Task<PagedResult<List<MediaDto>>> FetchAllProductMediaActivities([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingProductMediaActivity.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> media_ids = new List<Guid?>();            
            foreach(TrackingProductMediaActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                media_ids.Add(plt.product_media_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the product media details
            var medias = _context.ProductMedia.Where(x=>media_ids.Contains(x.product_media_id)).ToList();
            List<Guid?> product_ids = new List<Guid?>();            
            foreach(ProductMedia plt in medias){                 
                product_ids.Add(plt.product_id);                 
            }

            //fetch the product details
            var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();

            var utils = new TrackerUtils();

            List<MediaDto> details_final = new List<MediaDto>();
            foreach(TrackingProductMediaActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new MediaDto{
                    product_media_id = em.product_media_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(ProductMedia pm in medias){
                    if(pm.product_media_id == em.product_media_id){
                        foreach(ProductManagement user in products){
                            if(pm.product_id == user.product_id){
                                data.product_name = user.product_name;
                                data.product_id = user.product_id;
                            }
                    }
                    }
                }
                    
                

                details_final.Add(data);
            };

            return PagedResult<List<MediaDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("media/get/{id}/")]
        public async Task<PagedResult<List<MediaDto>>> FetchOneProductMediaActivities(Guid id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            //fetch all the product_media_ids from ProductMedia Table
            var medias = _context.ProductMedia.Where(x=>x.product_id==id).ToList();
            List<Guid?> media_ids = new List<Guid?>();            
            foreach(ProductMedia plt in medias){
                media_ids.Add(plt.product_media_id);                   
            }

            var email_x =await _context.TrackingProductMediaActivity.Where(x => media_ids.Contains(x.product_media_id)).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            // List<Guid?> media_ids = new List<Guid?>();            
            foreach(TrackingProductMediaActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                // media_ids.Add(plt.product_media_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            
            List<Guid?> product_ids = new List<Guid?>();            
            foreach(ProductMedia plt in medias){                 
                product_ids.Add(plt.product_id);                 
            }

            //fetch the product details
            var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();

            var utils = new TrackerUtils();

            List<MediaDto> details_final = new List<MediaDto>();
            foreach(TrackingProductMediaActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new MediaDto{
                    product_media_id = em.product_media_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(ProductMedia pm in medias){
                    if(pm.product_media_id == em.product_media_id){
                        foreach(ProductManagement user in products){
                            if(pm.product_id == user.product_id){
                                data.product_name = user.product_name;
                                data.product_id = user.product_id;
                            }
                    }
                    }
                }
                    
                

                details_final.Add(data);
            };

            return PagedResult<List<MediaDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("assignment/get/all/")]
        public async Task<PagedResult<List<AssignmentDto>>> FetchAllAssignmentActivities([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingProductPlantMapActivity.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> product_plant_mapping_ids = new List<Guid?>();            
            foreach(TrackingProductPlantMapActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                product_plant_mapping_ids.Add(plt.product_plant_mapping_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the product mapping details
            var mappings = _context.ProductPlantMapping.Where(x=>product_plant_mapping_ids.Contains(x.product_plant_mapping_id)).ToList();
            List<Guid?> product_ids = new List<Guid?>();   
            List<Guid?> plant_ids = new List<Guid?>();         
            foreach(ProductPlantMapping plt in mappings){                 
                product_ids.Add(plt.product_id);
                plant_ids.Add(plt.plant_id);                 
            }

            //fetch the product details
            var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();
            //fetch the plant details
            var plants = _context.Plant.Where(x=>plant_ids.Contains(x.plant_id)).ToList();

            var utils = new TrackerUtils();

            List<AssignmentDto> details_final = new List<AssignmentDto>();
            foreach(TrackingProductPlantMapActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new AssignmentDto{
                    product_plant_mapping_id = em.product_plant_mapping_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(ProductPlantMapping map in mappings){
                    if(map.product_plant_mapping_id == em.product_plant_mapping_id){
                        //add the plant details to the object
                        foreach(Plant plt in plants){
                            if(plt.plant_id == map.plant_id){
                                data.plant_id = plt.plant_id;
                                data.plant_name = plt.plant_name;
                                data.plant_code = plt.plant_code;
                            }
                        }

                        //add the product details to the object
                        foreach(ProductManagement plt in products){
                            if(plt.product_id == map.product_id){
                                data.product_id = plt.product_id;
                                data.product_name = plt.product_name;
                            }
                        }
                    }
                }
                    
                

                details_final.Add(data);
            };

            return PagedResult<List<AssignmentDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }




        [HttpPost("assignment/get/{plant_id}/")]
        public async Task<PagedResult<List<AssignmentDto>>> FetchOnePlantAssignmentActivities(Guid plant_id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            //fetch the product mapping details
            var mappings = _context.ProductPlantMapping.Where(x=>x.plant_id==plant_id).ToList();
            List<Guid?> product_plant_mapping_ids = new List<Guid?>(); 
            foreach(ProductPlantMapping map in mappings){
                product_plant_mapping_ids.Add(map.product_plant_mapping_id);
            }

            var email_x =await _context.TrackingProductPlantMapActivity.Where(x => product_plant_mapping_ids.Contains(x.product_plant_mapping_id)).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
                       
            foreach(TrackingProductPlantMapActivity plt in email){                 
                user_ids.Add(plt.user_id); 
                                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            
            List<Guid?> product_ids = new List<Guid?>();   
            List<Guid?> plant_ids = new List<Guid?>();         
            foreach(ProductPlantMapping plt in mappings){                 
                product_ids.Add(plt.product_id);
                plant_ids.Add(plt.plant_id);                 
            }

            //fetch the product details
            var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();
            //fetch the plant details
            var plants = _context.Plant.Where(x=>plant_ids.Contains(x.plant_id)).ToList();

            var utils = new TrackerUtils();

            List<AssignmentDto> details_final = new List<AssignmentDto>();
            foreach(TrackingProductPlantMapActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new AssignmentDto{
                    product_plant_mapping_id = em.product_plant_mapping_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(ProductPlantMapping map in mappings){
                    if(map.product_plant_mapping_id == em.product_plant_mapping_id){
                        //add the plant details to the object
                        foreach(Plant plt in plants){
                            if(plt.plant_id == map.plant_id){
                                data.plant_id = plt.plant_id;
                                data.plant_name = plt.plant_name;
                                data.plant_code = plt.plant_code;
                            }
                        }

                        //add the product details to the object
                        foreach(ProductManagement plt in products){
                            if(plt.product_id == map.product_id){
                                data.product_id = plt.product_id;
                                data.product_name = plt.product_name;
                            }
                        }
                    }
                }
                    
                

                details_final.Add(data);
            };

            return PagedResult<List<AssignmentDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }



        [HttpPost("qr/get/all/")]
        public async Task<PagedResult<List<QrDto>>> FetchAllQrActivities([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingQrManagement.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> qr_ids = new List<Guid?>();            
            foreach(TrackingQrManagement plt in email){                 
                user_ids.Add(plt.user_id); 
                qr_ids.Add(plt.qr_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the Qr details
            var qrs = _context.QrManagement.Where(x => qr_ids.Contains(x.qr_id)).ToList();
            
            List<Guid?> product_ids = new List<Guid?>();   
            List<Guid?> plant_ids = new List<Guid?>();         
            // foreach(ProductPlantMapping plt in mappings){                 
            //     product_ids.Add(plt.product_id);
            //     plant_ids.Add(plt.plant_id);                 
            // }

            //fetch the product details
            // var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();
            // //fetch the plant details
            // var plants = _context.Plant.Where(x=>plant_ids.Contains(x.plant_id)).ToList();

            var utils = new TrackerUtils();

            List<QrDto> details_final = new List<QrDto>();
            foreach(TrackingQrManagement em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                    if(changes.ContainsKey("product_id")){
                        product_ids.Add(new Guid(changes["produc_id"]["old_value"]));
                        product_ids.Add(new Guid(changes["produc_id"]["new_value"]));
                    }
                    if(changes.ContainsKey("plant_id")){
                        plant_ids.Add(new Guid(changes["plant_id"]["old_value"]));
                        plant_ids.Add(new Guid(changes["plant_id"]["new_value"]));
                    }
                }

                var data=new QrDto{
                    qr_id = em.qr_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }            
                

                details_final.Add(data);
            };

            //add the product or plant detisl
            if(product_ids.Count>0){
                var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();
                foreach(QrDto record in details_final){
                    if(record.changes.ContainsKey("product_id")){
                        foreach(ProductManagement product in products){
                            if(product.product_id==new Guid(record.changes["product_id"]["old_value"])){
                                record.changes["product_id"]["old_name"] = product.product_name;
                            }
                            if(product.product_id==new Guid(record.changes["product_id"]["new_value"])){
                                record.changes["product_id"]["new_name"] = product.product_name;
                            }
                        
                        }
                    }
                }
            }
            if(plant_ids.Count>0){
                var products = _context.Plant.Where(x=>plant_ids.Contains(x.plant_id)).ToList();
                foreach(QrDto record in details_final){
                    if(record.changes.ContainsKey("plant_id")){
                        foreach(Plant product in products){
                            if(product.plant_id==new Guid(record.changes["plant_id"]["old_value"])){
                                record.changes["plant_id"]["old_name"] = product.plant_name+" - "+product.plant_code;
                            }
                            if(product.plant_id==new Guid(record.changes["plant_id"]["new_value"])){
                                record.changes["plant_id"]["new_name"] =  product.plant_name+" - "+product.plant_code;
                            }
                        
                        }
                    }
                }
            }

            return PagedResult<List<QrDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }



        [HttpPost("qr/get/{id}/")]
        public async Task<PagedResult<List<QrDto>>> FetchOneQrActivities(Guid id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingQrManagement.Where(x => x.qr_id==id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> qr_ids = new List<Guid?>();            
            foreach(TrackingQrManagement plt in email){                 
                user_ids.Add(plt.user_id); 
                qr_ids.Add(plt.qr_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the Qr details
            var qrs = _context.QrManagement.Where(x => qr_ids.Contains(x.qr_id)).ToList();
            
            List<Guid?> product_ids = new List<Guid?>();   
            List<Guid?> plant_ids = new List<Guid?>();         
            // foreach(ProductPlantMapping plt in mappings){                 
            //     product_ids.Add(plt.product_id);
            //     plant_ids.Add(plt.plant_id);                 
            // }

            //fetch the product details
            // var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();
            // //fetch the plant details
            // var plants = _context.Plant.Where(x=>plant_ids.Contains(x.plant_id)).ToList();

            var utils = new TrackerUtils();

            List<QrDto> details_final = new List<QrDto>();
            foreach(TrackingQrManagement em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                    if(changes.ContainsKey("product_id")){
                        product_ids.Add(new Guid(changes["produc_id"]["old_value"]));
                        product_ids.Add(new Guid(changes["produc_id"]["new_value"]));
                    }
                    if(changes.ContainsKey("plant_id")){
                        plant_ids.Add(new Guid(changes["plant_id"]["old_value"]));
                        plant_ids.Add(new Guid(changes["plant_id"]["new_value"]));
                    }
                }

                var data=new QrDto{
                    qr_id = em.qr_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }            
                

                details_final.Add(data);
            };

            //add the product or plant detisl
            if(product_ids.Count>0){
                var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();
                foreach(QrDto record in details_final){
                    if(record.changes.ContainsKey("product_id")){
                        // if(products[0].product_id==new Guid(record.changes["product_id"]["old_value"])){
                        //     record.changes["product_id"]["old_name"] = products[0].product_name;
                        //     record.changes["product_id"]["new_name"] = products[1].product_name;
                        // }else{
                        //     record.changes["product_id"]["new_name"] = products[0].product_name;
                        //     record.changes["product_id"]["old_name"] = products[1].product_name;
                        // }
                        foreach(ProductManagement product in products){
                            if(product.product_id==new Guid(record.changes["product_id"]["old_value"])){
                                record.changes["product_id"]["old_name"] = product.product_name;
                            }
                            if(product.product_id==new Guid(record.changes["product_id"]["new_value"])){
                                record.changes["product_id"]["new_name"] = product.product_name;
                            }
                        
                        }
                    }
                }
            }
            if(plant_ids.Count>0){
                var products = _context.Plant.Where(x=>plant_ids.Contains(x.plant_id)).ToList();
                foreach(QrDto record in details_final){
                    if(record.changes.ContainsKey("plant_id")){
                        foreach(Plant product in products){
                            if(product.plant_id==new Guid(record.changes["plant_id"]["old_value"])){
                                record.changes["plant_id"]["old_name"] = product.plant_name+" - "+product.plant_code;
                            }
                            if(product.plant_id==new Guid(record.changes["plant_id"]["new_value"])){
                                record.changes["plant_id"]["new_name"] =  product.plant_name+" - "+product.plant_code;
                            }
                        
                        }
                    }
                }
            }

            return PagedResult<List<QrDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }




        [HttpPost("counterfeit/get/all/")]
        public async Task<PagedResult<List<CounterfietTrackerDto>>> FetchAllCounterfeitActivities([FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingCounterfeitManagement.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> counterfeit_mgmt_ids = new List<Guid?>();            
            foreach(TrackingCounterfeitManagement plt in email){                 
                user_ids.Add(plt.user_id); 
                counterfeit_mgmt_ids.Add(plt.counterfeit_mgmt_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the product mapping details
            var counterfiets = _context.CounterfietManagement.Where(x=>counterfeit_mgmt_ids.Contains(x.counterfeit_mgmt_id)).ToList();
            List<Guid?> product_ids = new List<Guid?>();       
            foreach(CounterfietManagement plt in counterfiets){                 
                product_ids.Add(plt.product_id);              
            }

            //fetch the product details
            var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();


            var utils = new TrackerUtils();

            List<CounterfietTrackerDto> details_final = new List<CounterfietTrackerDto>();
            foreach(TrackingCounterfeitManagement em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new CounterfietTrackerDto{
                    counterfeit_mgmt_id = em.counterfeit_mgmt_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(CounterfietManagement counter in counterfiets){
                    if(counter.counterfeit_mgmt_id == em.counterfeit_mgmt_id){
                        foreach(ProductManagement product in products){
                            if(counter.product_id == product.product_id){
                                data.product_id = product.product_id;
                                data.product_name = product.product_name;
                            }
                        }
                    }
                }
                    
                

                details_final.Add(data);
            };

            return PagedResult<List<CounterfietTrackerDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("counterfeit/get/{product_id}/")]
        public async Task<PagedResult<List<CounterfietTrackerDto>>> FetchOneProductCounterfeitActivities(Guid product_id,[FromQuery]PagingParams param)
        {
            var skip = (param.pageNumber - 1) * param.PageSize ;
            var counterfiets = _context.CounterfietManagement.Where(x=>x.product_id==product_id).ToList();
            List<Guid?> counterfeit_mgmt_ids = new List<Guid?>(); 
            foreach(CounterfietManagement plt in counterfiets){                 
                counterfeit_mgmt_ids.Add(plt.counterfeit_mgmt_id);              
            }

            
            var email_x =await _context.TrackingCounterfeitManagement.Where(x => counterfeit_mgmt_ids.Contains(x.counterfeit_mgmt_id)).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> counterfeit_mgmt_ids2 = new List<Guid?>();            
            foreach(TrackingCounterfeitManagement plt in email){                 
                user_ids.Add(plt.user_id); 
                counterfeit_mgmt_ids2.Add(plt.counterfeit_mgmt_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            //fetch the product mapping details
            counterfiets = counterfiets.Where(x=>counterfeit_mgmt_ids2.Contains(x.counterfeit_mgmt_id)).ToList();
            List<Guid?> product_ids = new List<Guid?>();       
            foreach(CounterfietManagement plt in counterfiets){                 
                product_ids.Add(plt.product_id);              
            }

            //fetch the product details
            var products = _context.ProductManagement.Where(x=>product_ids.Contains(x.product_id)).ToList();


            var utils = new TrackerUtils();

            List<CounterfietTrackerDto> details_final = new List<CounterfietTrackerDto>();
            foreach(TrackingCounterfeitManagement em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new CounterfietTrackerDto{
                    counterfeit_mgmt_id = em.counterfeit_mgmt_id,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                foreach(CounterfietManagement counter in counterfiets){
                    if(counter.counterfeit_mgmt_id == em.counterfeit_mgmt_id){
                        foreach(ProductManagement product in products){
                            if(counter.product_id == product.product_id){
                                data.product_id = product.product_id;
                                data.product_name = product.product_name;
                            }
                        }
                    }
                }                
                

                details_final.Add(data);
            };

            return PagedResult<List<CounterfietTrackerDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("user/get/all/")]
        public async Task<PagedResult<List<UserTrackerDto>>> FetchAlluserActivities([FromQuery]PagingParams param)
        {
            // TODO : need to update the logic
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingUserEditActivity.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> plant_ids = new List<Guid?>();            
            foreach(TrackingUserEditActivity plt in email){                 
                user_ids.Add(plt.edited_by);
                user_ids.Add(plt.user_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            
            var utils = new TrackerUtils();

            List<UserTrackerDto> details_final = new List<UserTrackerDto>();
            foreach(TrackingUserEditActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new UserTrackerDto{
                    edited_by_id = em.edited_by,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                    if(em.edited_by == user.user_id){
                        data.edited_by_name = user.full_name;
                    }
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<UserTrackerDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }

        [HttpPost("user/get/{id}/")]
        public async Task<PagedResult<List<UserTrackerDto>>> FetchOneuserActivities(Guid id,[FromQuery]PagingParams param)
        {
            // TODO : need to update the logic
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingUserEditActivity.Where(x => x.edited_by==id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();
            List<Guid?> plant_ids = new List<Guid?>();            
            foreach(TrackingUserEditActivity plt in email){                 
                user_ids.Add(plt.edited_by);
                user_ids.Add(plt.user_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            
            var utils = new TrackerUtils();

            List<UserTrackerDto> details_final = new List<UserTrackerDto>();
            foreach(TrackingUserEditActivity em in email){
                String action= "create";
                Dictionary<String, Dictionary<String,String>> changes = new Dictionary<String, Dictionary<String,String>> ();

                if(em.old_obj!="{}"){                    
                    changes = utils.FindChanges(em.old_obj,em.new_obj);
                    if(changes.ContainsKey("status")){
                        action = "status_change";
                    }else{
                        action = "edit";
                    }
                }

                var data=new UserTrackerDto{
                    edited_by_id = em.edited_by,
                    action = action,
                    changes = changes,
                    user_id = em.user_id
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                    if(em.edited_by == user.user_id){
                        data.edited_by_name = user.full_name;
                    }
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<UserTrackerDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }



        [HttpPost("activity/get/all/")]
        public async Task<PagedResult<List<TrackingActivityDto>>> FetchAllActivities([FromQuery]PagingParams param)
        {
            // TODO : need to update the logic
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingActivity.Where(x => true).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            List<Guid?> user_ids = new List<Guid?>();           
            foreach(TrackingActivity plt in email){
                user_ids.Add(plt.user_id);                   
            }

            //fetch the user details
            var users = _context.User.Where(x=> user_ids.Contains(x.user_id)).ToList();
            
            var utils = new TrackerUtils();

            List<TrackingActivityDto> details_final = new List<TrackingActivityDto>();
            foreach(TrackingActivity em in email){

                var data=new TrackingActivityDto{
                    message = em.message,
                    severity_type = em.severity_type.ToString(),
                    user_id = em.user_id,
                    created_at = em.created_at
                    };

                foreach(User user in users){
                    if(em.user_id == user.user_id){
                        data.user_name = user.full_name;
                    }
                }

                

                details_final.Add(data);
            };

            return PagedResult<List<TrackingActivityDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }


        [HttpPost("activity/get/{id}/")]
        public async Task<PagedResult<List<TrackingActivityDto>>> FetchOneUserActivities(Guid id,[FromQuery]PagingParams param)
        {
            // TODO : need to update the logic
            var skip = (param.pageNumber - 1) * param.PageSize ;

            var email_x =await _context.TrackingActivity.Where(x => x.user_id == id).ToListAsync();
            var count = email_x.Count;

            //sort the records in descending order and fetch the product ids
            var email = email_x.OrderByDescending(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            if(!param.Sort){
                email = email_x.OrderBy(x => x.created_at).Skip(skip).Take(param.PageSize).ToList();
            }

            //fetch the user details
            var users = _context.User.Where(x=> x.user_id==id).ToList();
            
            var utils = new TrackerUtils();

            List<TrackingActivityDto> details_final = new List<TrackingActivityDto>();
            foreach(TrackingActivity em in email){

                var data=new TrackingActivityDto{
                    message = em.message,
                    severity_type = em.severity_type.ToString(),
                    user_id = em.user_id,
                    created_at = em.created_at,
                    user_name = users[0].full_name
                    };                

                details_final.Add(data);
            };

            return PagedResult<List<TrackingActivityDto>>.Success(details_final,param.pageNumber,param.PageSize,count);

        }



        
    }
}