namespace Domain.Pipeline
{
    using System.Collections.Generic;
    using MediatR;

    public class CommandResult
    {
        readonly List<INotification> notifications;

        CommandResult()
        {
            notifications = new List<INotification>();
        }

        public CommandResult WithNotification(INotification notification)
        {
            notifications.Add(notification);
            return this;
        }

        public IReadOnlyCollection<INotification> GetNotifications()
        {
            return notifications.AsReadOnly();
        }

        public static CommandResult Void => new CommandResult();
    }
}