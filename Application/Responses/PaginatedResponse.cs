namespace Application.Responses
{
    public class PaginatedResponse<T> where T : class
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int? Page { get; set; }
        public int? Size { get; set; }

        public PaginatedResponse(IEnumerable<T> data, int totalCount, int? page, int? size)
        {
            Data = data;
            TotalCount = totalCount;
            Page = page;
            Size = size;
        }
    }
}
