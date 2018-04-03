namespace Domain.Features.Category
{
    using MediatR;
    using Pipeline;

    public class Create
    {
        public class Command : IRequest<CommandResult>
        {
            public string Name { get; set; }
            public string Location { get; set; }
        }
    }
}