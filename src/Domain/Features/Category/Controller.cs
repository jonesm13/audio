namespace Domain.Features.Category
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("categories")]
    public class CategoryController : ApiController
    {
        // [HttpGet, Route("")]

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] Create.Command command) => await NoContent(Mediator.Send(command));

        [HttpDelete, Route("")]
        public async Task<IHttpActionResult> Delete([FromBody] Delete.Command command) => await NoContent(Mediator.Send(command));
    }
}