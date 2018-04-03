namespace Domain.Pipeline
{
    using System.Threading.Tasks;
    using DataModel;
    using MediatR;

    public abstract class EntityFrameworkCommandHandler<TRequest, TResult>
        : AsyncRequestHandler<TRequest, TResult> where TRequest : IRequest<TResult>
    {
        protected AudioDbContext Db { get; }

        protected EntityFrameworkCommandHandler(AudioDbContext db)
        {
            Db = db;
        }

        protected abstract Task<TResult> HandleImpl(TRequest request);

        protected override async Task<TResult> HandleCore(TRequest request)
        {
            TResult result = await HandleImpl(request);

            await Db.SaveChangesAsync();

            return result;
        }
    }
}
