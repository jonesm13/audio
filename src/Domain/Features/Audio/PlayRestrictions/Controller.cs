namespace Domain.Features.Audio.PlayRestrictions
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("audio/{id}/play-restrictions")]
    public class AudioPlayRestrictionsController : ApiController
    {
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Add(Add.Command command) => await NoContent(Mediator.Send(command));

        [HttpDelete, Route("{restrictionId}")]
        public async Task<IHttpActionResult> Remove(Remove.Command command) => await NoContent(Mediator.Send(command));
    }
}