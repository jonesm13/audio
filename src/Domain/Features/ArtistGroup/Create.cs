namespace Domain.Features.ArtistGroup
{
    using MediatR;
    using Pipeline;

    public class Create
    {
        public class Command : IRequest<CommandResult>
        {
            public string Name { get; set; }
            public string[] Members { get; set; }
        }
    }
}