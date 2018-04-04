namespace Domain.Settings
{
    using System.Configuration;

    public class Settings
    {
        public class Inbox
        {
            public static string InboxFolder => ConfigurationManager.AppSettings["audio:inboxPath"];
        }
    }
}