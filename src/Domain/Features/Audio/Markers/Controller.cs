namespace Domain.Features.Audio.Markers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("audio/{id}/markers")]
    public class AudioMarkersController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Create(Create.Command command) => await NoContent(Mediator.Send(command));
    }
}