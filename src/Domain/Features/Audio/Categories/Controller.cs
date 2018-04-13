namespace Domain.Features.Audio.Categories
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("audio/{id}/categories")]
    public class AudioCategoriesController : ApiController
    {
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Add(Add.Command command) => await NoContent(Mediator.Send(command));

        [HttpDelete, Route("")]
        public async Task<IHttpActionResult> Remove(Remove.Command command) => await NoContent(Mediator.Send(command));
    }
}