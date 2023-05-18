using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class RangeTable
    {
        [Key]
        public Guid Id { get; set; }
        public String last_used_value { get; set; } = "0000000000";

    }
}