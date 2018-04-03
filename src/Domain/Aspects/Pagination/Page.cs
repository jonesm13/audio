namespace Domain.Aspects.Pagination
{
    using System.Collections.Generic;

    public class Page<T>
    {
        public IEnumerable<T> Data { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        public Page(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = data;
        }
    }
}
