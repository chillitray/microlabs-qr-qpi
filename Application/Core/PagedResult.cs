namespace Application.Core
{
    public class PagedResult<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        // public bool IsSuccess { get; set; }
        public T Value { get; set; }
        
        // public string Error { get; set; }

        public static PagedResult<T> Success(T value, int pageNumber, int pageSize, int count) => new PagedResult<T> { 
            // IsSuccess = true, 
            Value=value,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = count
        };
        public static PagedResult<T> Failure(string error) => new PagedResult<T>{};

        // public PagedResult(T data, int pageNumber, int pageSize)
        // {
        //     this.PageNumber = pageNumber;
        //     this.PageSize = pageSize;
        //     this.Value = data;
        //     this.IsSuccess = true;
        //     this.Error = null;
        // }
    }
}