using System.Linq;
using Template.Common.Notifications;

namespace Template.Models.ServiceModels
{
    public class ServiceResponse : IServiceResponse
    {
        public NotificationCollection Notifications { get; set; }

        public bool IsSuccessful { get { return !Notifications.Any(n => n.Type == NotificationTypeEnum.Error); } }

        public ServiceResponse()
        {
            Notifications = new NotificationCollection();
        }
    }
}
