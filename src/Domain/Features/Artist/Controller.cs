namespace Domain.Features.Artist
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("artists")]
    public class ArtistController : ApiController
    {
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create(Create.Command command) => await NoContent(Mediator.Send(command));
    }
}