using WEB_DAU_GIA.Models.Entities;

namespace WEB_DAU_GIA.Models.ViewModels
{
    public class NotificationViewModel
    {
        public List<Notification> NewNotifications { get; set; } = new();
        public List<Notification> OlderNotifications { get; set; } = new();
        public int UnreadCount { get; set; }
    }
}
