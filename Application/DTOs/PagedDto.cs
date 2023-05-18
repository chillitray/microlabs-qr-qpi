using System.Collections;

namespace Application.DTOs
{
    public class PagedDto
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable items { get; set; }
    }
}