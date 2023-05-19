using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<User> userManager)
        {
            
            // var activities = new List<Plant>
            // {
            //     new Plant
            //     {
            //         plant_name = "Bangalore Plant",
            //         plant_code = "PLNT002",
            //         plant_description = "This a second temporary plant located in Bngalore",
            //         plant_location_address = "RT Nagar",
            //         plant_location_city = "Bangalore",
            //         plant_location_state = "Karanataka",
            //         plant_location_country = "India",
            //         plant_location_pincode = "516320",
            //         plant_location_geo="temp geo",
            //         plant_qr_limit = 3000,
            //         created_by = new Guid("930e0d95-7a3f-4bb5-abc3-08db5165c970"),
            //         operated_id = new Guid("9fd2e83f-7258-4e2c-7140-08db5166684a"),
            //         founded_on = DateTime.Now
            //     }
            // };

            // await context.Plant.AddRangeAsync(activities);
            // await context.SaveChangesAsync();


            // var products = new List<ProductManagement>
            // {
            //     new ProductManagement
            //     {
            //         product_name = "Dolo-650",
            //         product_description = "Dolo-650 product description",
            //         product_logo="temp_logo",
            //         product_writeup="temp writeup",
            //         product_expiry_days=100,
            //         product_mrp=50,
            //         created_by = new Guid("1093e3a1-8170-4004-9385-1994f6715536")
            //     }
            // };

            // await context.ProductManagement.AddRangeAsync(products);
            // Console.WriteLine("Hello");
            // var users = new List<User>
            // {
                
            //     new User
            //     {
            //         Email = "ravi@zintlr.com",
            //         emp_id = "EMP002",
            //         full_name = "Ravi Jain",
            //         role_id = new Guid("4f99b3c5-48ea-47dd-502a-08db56d63737"),
            //         UserName = "ravi",
            //         created_by=new Guid("1093e3a1-8170-4004-9385-1994f6715536")
            //     },
            //     new User
            //     {
            //         Email = "srinidhi@zintlr.com",
            //         emp_id = "EMP003",
            //         full_name = "Srinidhi Athreyas",
            //         role_id = new Guid("4f99b3c5-48ea-47dd-502a-08db56d63737"),
            //         UserName = "srinidhi",
            //         created_by=new Guid("1093e3a1-8170-4004-9385-1994f6715536")
            //     },
            //     new User
            //     {
            //         Email = "dipak@zintlr.com",
            //         emp_id = "EMP004",
            //         full_name = "Dipak Patil",
            //         role_id = new Guid("4f99b3c5-48ea-47dd-502a-08db56d63737"),
            //         UserName = "dipak",
            //         created_by=new Guid("1093e3a1-8170-4004-9385-1994f6715536")
            //     },
            //     new User
            //     {
            //         Email = "ujwal@zintlr.com",
            //         emp_id = "EMP005",
            //         full_name = "Ujwal Kumar",
            //         role_id = new Guid("4f99b3c5-48ea-47dd-502a-08db56d63737"),
            //         UserName = "ujwal",
            //         created_by=new Guid("1093e3a1-8170-4004-9385-1994f6715536")
            //     },
            //     new User
            //     {
            //         Email = "sana@zintlr.com",
            //         emp_id = "EMP005",
            //         full_name = "Sana Ismail",
            //         role_id = new Guid("4f99b3c5-48ea-47dd-502a-08db56d63737"),
            //         UserName = "sana",
            //         created_by=new Guid("1093e3a1-8170-4004-9385-1994f6715536")
            //     }
            // };
            // foreach(User user in users){
            //         Console.WriteLine("Hello234");
            //         var result = await userManager.CreateAsync(user, "Pa$$w0rd");
            //         Console.WriteLine(result);
            // }

            // var roles = new List<Role>{
            //     new Role
            //     {
            //         role = "Admin",
            //         access_level = AccessLevelOptions.ADMIN,
            //         // created_by = new Guid("930e0d95-7a3f-4bb5-abc3-08db5165c970")
            //     }
            // };
            // await context.Role.AddRangeAsync(roles);

            // var mapping = new List<ProductPlantMapping>{
            //     new ProductPlantMapping
            //     {
            //         product_id=new Guid("e5882838-c97b-407a-3672-08db51666845"),
            //         plant_id=new Guid("7f2d5b03-005b-439b-d312-08db51666833"),
            //         created_by=new Guid("930e0d95-7a3f-4bb5-abc3-08db5165c970")
            //     }
            // };
            // await context.ProductPlantMapping.AddRangeAsync(mapping);

            // var mapping = new List<RateLimits>{
            //     new RateLimits
            //     {
            //         rate_type = RateTypeOptions.MAX_LOGIN_FAILED_ATTEMPTS,
            //         max_allowed_per_day = 3,
            //         created_by=new Guid("1093e3a1-8170-4004-9385-1994f6715536")
            //     }
            // };
            // await context.RateLimits.AddRangeAsync(mapping);

            // var mapping = new List<RangeTable>{
            //     new RangeTable
            //     {
            //         last_used_value = "0000000000"
            //     }
            // };
            // await context.RangeTable.AddRangeAsync(mapping);


            await context.SaveChangesAsync();
        }
    }
}