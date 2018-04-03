namespace Infrastructure.WebApi
{
    using System.Net;
    using System.Threading.Tasks;
    using System.Web.Http;
    using MediatR;

    public abstract class ApiController : System.Web.Http.ApiController
    {
        public IMediator Mediator { protected get; set; }

        protected async Task<IHttpActionResult> Ok<T>(Task<T> contentTask) => Ok(await contentTask);

        protected IHttpActionResult NoContent() => StatusCode(HttpStatusCode.NoContent);

        protected async Task<IHttpActionResult> NoContent(params Task[] tasks)
        {
            await Task.WhenAll(tasks);
            return NoContent();
        }
    }
}
