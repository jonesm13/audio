namespace Domain.Features.Audio
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("audio")]
    public class AudioController : ApiController
    {
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> Index(Index.Query query) => await Ok(Mediator.Send(query ?? new Index.Query()));

        [HttpGet, Route("{id}")]
        public async Task<IHttpActionResult> Index(Detail.Query query) => await Ok(Mediator.Send(query ?? new Detail.Query()));

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Load(Load.Command command) => await NoContent(Mediator.Send(command));

        [HttpPut, Route("{id}")]
        public async Task<IHttpActionResult> Replace(Replace.Command command) => await NoContent(Mediator.Send(command));

        [HttpDelete, Route("{audioId}")]
        public async Task<IHttpActionResult> Delete([FromUri] Delete.Command command) => await NoContent(Mediator.Send(command));
    }
}