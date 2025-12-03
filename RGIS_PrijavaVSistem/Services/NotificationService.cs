using System;
using System.Linq;

namespace BookTracker.Domain
{
    public interface INotificationService
    {
        // US18 – kličeš po osvežitvi napredka
        MilestoneNotification? CheckMilestones(Guid userId, int year);
    }

    public class NotificationService : INotificationService
    {
        private readonly InMemoryDataStore _db;
        private readonly IReadingGoalService _goals;

        public NotificationService(InMemoryDataStore db, IReadingGoalService goals)
        {
            _db = db;
            _goals = goals;
        }

        public MilestoneNotification? CheckMilestones(Guid userId, int year)
        {
            var (completed, target, percent) = _goals.GetYearProgress(userId, year);
            if (target == 0) return null;

            string? msg = null;

            if (percent >= 100 && !_db.Notifications.Any(n => n.UserId == userId && n.Message.Contains("100%")))
                msg = "Čestitke! Dosegel si 100% bralnega cilja.";
            else if (percent >= 50 && !_db.Notifications.Any(n => n.UserId == userId && n.Message.Contains("50%")))
                msg = "Super! Dosegel si 50% bralnega cilja.";

            if (msg == null) return null;

            var notification = new MilestoneNotification
            {
                UserId = userId,
                Message = msg
            };
            _db.Notifications.Add(notification);
            return notification;
        }
    }
}
