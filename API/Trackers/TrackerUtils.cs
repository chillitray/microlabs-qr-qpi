using System.Reflection;
using API.DTOs;
using API.Services;
using Domain;

namespace API.Trackers
{
    public class TrackerUtils
    {

        public String ConvertToString<T>(T new_obj){
            //convert the record into dictionary
            // Console.WriteLine(new_obj);
            var new_obj2 = new_obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop =>prop.GetValue(new_obj, null).ToString());

            // Console.WriteLine("Hello2");
            var utils = new Utils();
            // convert into string
            var new_obj_string = utils.DictionaryToString(new_obj2);

            return new_obj_string;

        }

        public Dictionary<String,Dictionary<String,String>> FindChanges(String old_obj , String new_obj){
            //convert the string into dictionary
            var old_obj_dict = old_obj.Remove(0,1).Remove(old_obj.Length-2).Split(',').ToDictionary(e=>e.Split(':')[0].Trim(), e=>e.Split(':')[1].Trim());
            var new_obj_dict = new_obj.Remove(0,1).Remove(new_obj.Length-2).Split(',').ToDictionary(e=>e.Split(':')[0].Trim(), e=>e.Split(':')[1].Trim());

            //compare the two strings and return the changes
            // var dict3 = dict2.Where(entry => dict1[entry.Key] != entry.Value)
            //      .ToDictionary(entry => entry.Key, entry => entry.Value);

            //find the keys that have different values
            var keysWithDifferentValues = new List<string>();
            foreach (var kvp in old_obj_dict)
            {
                if(kvp.Key == "last_updated_at" | kvp.Key == "created_by" | kvp.Key == "created_at") continue;
                
                if(!kvp.Value.Equals(new_obj_dict[kvp.Key]))
                    keysWithDifferentValues.Add(kvp.Key);
            }

            Dictionary<String,Dictionary<String,String>> final_dict = new Dictionary<string, Dictionary<String,String>>();
            foreach(String key in keysWithDifferentValues){
                final_dict[key] = new Dictionary<String,String>{
                    {"old_value",old_obj_dict[key]},
                    {"new_value",new_obj_dict[key]}
                };
            }
            return final_dict;

        }

        public String CreateCounterfietObj(CounterfietManagement record){
            var new_obj = new CounterfietManagement{
                counterfeit_mgmt_id = record.counterfeit_mgmt_id,
                product_id = record.product_id,
                counterfeit_type = record.counterfeit_type,
                low_risk_threshold = record.low_risk_threshold,
                moderate_threshold = record.moderate_threshold,
                high_risk_threshold = record.high_risk_threshold,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at
            };


            return this.ConvertToString(new_obj);
        }

        public String CreateSmtpObj(SmtpConfig record){
            var new_obj = new SmtpConfig{
                smtp_config_id = record.smtp_config_id,
                max_emails_per_day = record.max_emails_per_day,
                email_id = record.email_id,
                password = record.password,
                email_type = record.email_type,
                status = record.status,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at
            };

            return this.ConvertToString(new_obj);
        }

        public String CreateNotificationObj(Notification record){
            var new_obj = new Notification{
                notification_id = record.notification_id,
                notification_content = record.notification_content,
                notification_type = record.notification_type,
                redirect_url = record.redirect_url,
                status = record.status,
                created_at = record.created_at,
                created_by = record.created_by,
                last_updated_at = record.last_updated_at
            };
            return this.ConvertToString(new_obj);
        }

        public String CreateNotificationTypeObj(NotificationTypeManagement record){
            var new_obj = new NotificationTypeManagement{
                notification_type_id = record.notification_type_id,
                notifiication_type = record.notifiication_type,
                priority = record.priority,
                notification_for = record.notification_for,
                status = record.status,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at
            };
            return this.ConvertToString(new_obj);
        }

        public String CreatePlantActivityObj(Plant record){
            var new_obj = new Plant{
                plant_id = record.plant_id,
                plant_name = record.plant_name,
                plant_code = record.plant_code,
                plant_description = record.plant_description,
                plant_location_address = record.plant_location_address,
                plant_location_city = record.plant_location_city,
                plant_location_state = record.plant_location_state,
                plant_location_country = record.plant_location_country,
                plant_location_pincode = record.plant_location_pincode,
                plant_location_geo = record.plant_location_geo,
                plant_qr_limit = record.plant_qr_limit,
                created_by = record.created_by,
                operated_id = record.operated_id,
                status = record.status,
                founded_on = record.founded_on,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at
            };
            return this.ConvertToString(new_obj);
        }

        public String CreatePlantKeyObj(PlantKeyManagement record){
            var new_obj = new PlantKeyManagement{
                plant_key_id = record.plant_key_id,
                plant_id = record.plant_id,
                plant_key = record.plant_key,
                status = record.status,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at
            };
            return this.ConvertToString(new_obj);
        }

        public String CreateProductObj(ProductManagement record){
            var new_obj = new ProductManagement{
                product_id = record.product_id,
                product_name = record.product_name,
                product_description = record.product_description,
                product_logo = record.product_logo,
                product_writeup = record.product_writeup,
                product_expiry_days = record.product_expiry_days,
                product_mrp = record.product_mrp,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at,
                status = record.status
            };
            return this.ConvertToString(new_obj);
        }

        public String CreateProductMediaObj(ProductMedia record){
            var new_obj = new ProductMedia{
                product_media_id = record.product_media_id,
                url = record.url,
                media_type = record.media_type,
                product_id = record.product_id,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at,
                status = record.status

            };
            return this.ConvertToString(new_obj);
        }

        public String CreateQrManagementObj(AddQrTrackerDto record){
            var new_obj = new AddQrTrackerDto{
                qr_id = record.qr_id,
                // product_id = record.product_id,
                // plant_id = record.plant_id,
                // public_id = record.public_id,
                // manufactured_date = record.manufactured_date,
                // expiry_date = record.expiry_date,
                // product_mrp_copy = record.product_mrp_copy,
                // pack_id = record.pack_id,
                // serial_number = record.serial_number,
                // batch_no = record.batch_no,
                status = record.status,
                // created_at_ip = record.created_at_ip,
                // updated_by = record.updated_by,
                // created_at = record.created_at,
                // last_updated_at = record.last_updated_at
            };
            return this.ConvertToString(new_obj);
        }

        public String CreateRateLimitObj(RateLimits record){
            var new_obj = new RateLimits{
                rate_limit_id = record.rate_limit_id,
                rate_type = record.rate_type,
                max_allowed_per_day = record.max_allowed_per_day,
                max_allowed_overall = record.max_allowed_overall,
                status = record.status,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at
            };
            return this.ConvertToString(new_obj);
        }

        public String CreateUserbj(AddUserTrackerDto record){
            var new_obj = new AddUserTrackerDto{
                user_id = record.user_id,
                Email = record.Email,
                emp_id = record.emp_id,
                joined_date = record.joined_date,
                last_updated_at = record.last_updated_at,
                full_name = record.full_name,
                role_id = record.role_id,
                status = record.status,
                plant_id = record.plant_id,
                created_at = record.created_at,
                created_by = record.created_by
            };
            return this.ConvertToString(new_obj);
        }
    }
}