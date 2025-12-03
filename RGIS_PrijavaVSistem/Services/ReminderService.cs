using System;
using System.Linq;

namespace BookTracker.Domain
{
    public interface IReminderService
    {
        ReminderSettings SetReminder(Guid userId, TimeSpan timeOfDay);   // US15
        void DisableReminder(Guid userId);
        ReminderSettings? GetReminder(Guid userId);
    }

    public class ReminderService : IReminderService
    {
        private readonly InMemoryDataStore _db;

        public ReminderService(InMemoryDataStore db)
        {
            _db = db;
        }

        public ReminderSettings SetReminder(Guid userId, TimeSpan timeOfDay)
        {
            var r = _db.Reminders.SingleOrDefault(x => x.UserId == userId);
            if (r == null)
            {
                r = new ReminderSettings { UserId = userId };
                _db.Reminders.Add(r);
            }

            r.TimeOfDay = timeOfDay;
            r.Enabled = true;
            return r;
        }

        public void DisableReminder(Guid userId)
        {
            var r = _db.Reminders.SingleOrDefault(x => x.UserId == userId);
            if (r != null) r.Enabled = false;
        }

        public ReminderSettings? GetReminder(Guid userId) =>
            _db.Reminders.SingleOrDefault(x => x.UserId == userId);
    }
}
