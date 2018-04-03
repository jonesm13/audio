namespace Domain.Aspects.Pagination
{
    using System.Linq;

    public static class PaginationExtensions
    {
        public static PaginatedQueryable<T> InPagesOf<T>(
            this IQueryable<T> source,
            int pageSize)
        {
            return new PaginatedQueryable<T>(source, pageSize);
        }
    }
}