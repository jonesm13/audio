namespace Domain.Aspects.Pagination
{
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;

    public class PaginatedQueryable<T>
    {
        readonly IQueryable<T> source;
        readonly int pageSize;

        public PaginatedQueryable(IQueryable<T> source, int pageSize)
        {
            Ensure.IsNotNull(source, nameof(source));
            Ensure.IsPositiveInteger(pageSize, nameof(pageSize));

            this.source = source;
            this.pageSize = pageSize;
        }

        public Task<Page<T>> ToPageAsync(int number)
        {
            int start = (number - 1) * pageSize;

            return Task.FromResult(
                new Page<T>(
                    source.Skip(start).Take(pageSize),
                    number,
                    pageSize,
                    source.Count()));
        }
    }
}