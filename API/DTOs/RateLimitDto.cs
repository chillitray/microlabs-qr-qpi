namespace API.DTOs
{
    public class RateLimitDto
    {
        public Guid? rate_limit_id { get; set; }
        public String rate_type { get; set; }
        public int max_allowed_per_day { get; set; }
        public int? max_allowed_overall { get; set; }
        public String status { get; set; }

        public DateTime? created_at { get; set; } = DateTime.Now;
    }
}