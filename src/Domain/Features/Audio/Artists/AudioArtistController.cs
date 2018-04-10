namespace Domain.Features.Audio.Artists
{
    using System.Web.Http;
    using ApiController = Infrastructure.WebApi.ApiController;

    [RoutePrefix("audio/{id}/artists")]
    public class AudioArtistController : ApiController
    {
    }
}