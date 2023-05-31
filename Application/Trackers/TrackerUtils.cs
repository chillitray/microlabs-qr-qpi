using System.Reflection;
using Application.Services;
using Domain;

namespace Application.Trackers
{
    public class TrackerUtils
    {

        public String ConvertToString<T>(T new_obj){
             //convert the record into dictionary
            var new_obj2 = new_obj.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .ToDictionary(prop => prop.Name, prop =>prop.GetValue(new_obj, null).ToString());

            var utils = new Utils();
            // convert into string
            var new_obj_string = utils.DictionaryToString(new_obj2);

            return new_obj_string;

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

        public String CreateProductPlantmappingObj(ProductPlantMapping record){
            var new_obj = new ProductPlantMapping{
                product_plant_mapping_id= record.product_plant_mapping_id,
                product_id=record.product_id,
                plant_id = record.plant_id,
                status = record.status,
                created_by = record.created_by,
                created_at = record.created_at,
                last_updated_at = record.last_updated_at
            };
            return this.ConvertToString(new_obj);
        }
    }
}