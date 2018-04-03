namespace Domain.Features.Health
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("health")]
    public class HealthController : ApiController
    {
        [HttpGet, Route("ping")]
        public async Task<IHttpActionResult> Ping(Get.Query query) => await Ok(Mediator.Send(query ?? new Get.Query()));
    }
}
