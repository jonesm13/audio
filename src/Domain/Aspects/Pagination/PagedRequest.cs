namespace Domain.Aspects.Pagination
{
    using MediatR;

    public class PagedRequest<T> : IRequest<Page<T>>
    {
        public int PageNumber { get; } = 1;
        public int PageSize { get; } = 20;
    }
}
