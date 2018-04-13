namespace Domain.Features.Audio.Artists
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("audio/{id}/artists")]
    public class AudioArtistController : ApiController
    {
        [HttpPost, Route("{name}")]
        public async Task<IHttpActionResult> Add([FromUri] Add.Command command) => await NoContent(Mediator.Send(command));
    }
}