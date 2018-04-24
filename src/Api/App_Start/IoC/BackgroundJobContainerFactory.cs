namespace Api.IoC
{
    using SimpleInjector;

    public class BackgroundJobContainerFactory
    {
        public static Container Create()
        {
            Container result = new Container();

            return result;
        }
    }
}