namespace Domain.Features.Health
{
    using System;
    using System.Threading.Tasks;
    using MediatR;

    public class Get
    {
        public class Query : IRequest<Model>
        {
        }

        public class Handler : AsyncRequestHandler<Query, Model>
        {
            protected override Task<Model> HandleCore(Query request)
            {
                return Task.FromResult(new Model
                {
                    ServerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                });
            }
        }

        public class Model
        {
            public long ServerTime { get; set; }
        }
    }
}
