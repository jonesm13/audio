namespace Domain.Features.Artist
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("artists")]
    public class ArtistController : ApiController
    {
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Index(Index.Query query) => await Ok(Mediator.Send(query ?? new Index.Query()));

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(Create.Command command) => await NoContent(Mediator.Send(command));
    }
}