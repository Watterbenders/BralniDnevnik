using System.Collections.Generic;

namespace BookTracker.Domain
{
    public class InMemoryDataStore
    {
        public List<User> Users { get; } = new();
        public List<Book> Books { get; } = new();
        public List<BookQuote> Quotes { get; } = new();
        public List<ReadingGoal> Goals { get; } = new();
        public List<ReadingProgress> Progresses { get; } = new();
        public List<ReminderSettings> Reminders { get; } = new();
        public List<Challenge> Challenges { get; } = new();
        public List<MilestoneNotification> Notifications { get; } = new();
        public List<Recommendation> Recommendations { get; } = new();
        public List<Comment> Comments { get; } = new();
        public List<ThemeSettings> Themes { get; } = new();
        public List<ReadingProfile> Profiles { get; } = new();
    }
}
