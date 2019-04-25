using Template.Common.Notifications;

namespace Template.Models.ServiceModels
{
    public interface IServiceResponse
    {
        NotificationCollection Notifications { get; set; }
    }
}
