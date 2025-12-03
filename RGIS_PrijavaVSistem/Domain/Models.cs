using System;
using System.Collections.Generic;

namespace BookTracker.Domain
{
    public enum BookStatus
    {
        NaCakanju,
        VBranju,
        Prebrana
    }

    public enum ThemeMode
    {
        Light,
        Dark
    }

    public enum LeaderboardPeriod
    {
        Day,
        Week,
        Month,
        Year
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // za demo, v realnosti hash
        public string DisplayName { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = true;
    }

    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OwnerId { get; set; }

        public string Title { get; set; } = string.Empty;    // US1,6
        public string Author { get; set; } = string.Empty;   // US1,6
        public string Genre { get; set; } = string.Empty;    // US1,8,22,23
        public int Pages { get; set; }                       // US1,12,21,22
        public BookStatus Status { get; set; } = BookStatus.NaCakanju; // US2,8
        public string Notes { get; set; } = string.Empty;    // US3
        public int? Rating { get; set; }                     // US9
        public string? CoverPath { get; set; }               // US5

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }            // za cilje, statistiko
    }

    public class BookQuote
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BookId { get; set; }                     // US4
        public string Text { get; set; } = string.Empty;
    }

    public class ReadingGoal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }                     // US11,13,18
        public int Year { get; set; }
        public int TargetBooks { get; set; }
    }

    public class ReadingProgress
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BookId { get; set; }                     // US12
        public int PagesRead { get; set; }
    }

    public class ReminderSettings
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }                     // US15
        public TimeSpan TimeOfDay { get; set; }
        public bool Enabled { get; set; }
    }

    public class Challenge
    {
        public Guid Id { get; set; } = Guid.NewGuid();       // US16
        public Guid UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool Completed { get; set; }
    }

    public class MilestoneNotification
    {
        public Guid Id { get; set; } = Guid.NewGuid();       // US18
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Recommendation
    {
        public Guid Id { get; set; } = Guid.NewGuid();       // US19,26
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public Guid BookId { get; set; }
        public string? Note { get; set; }
    }

    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();       // US27
        public Guid BookId { get; set; }
        public Guid AuthorId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ThemeSettings
    {
        public Guid Id { get; set; } = Guid.NewGuid();       // US29,30
        public Guid UserId { get; set; }
        public ThemeMode Mode { get; set; } = ThemeMode.Light;
        public string? PrimaryColor { get; set; }
        public string? BackgroundImage { get; set; }
        public string? FontFamily { get; set; }
    }

    public class ReadingProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();       // US24
        public Guid UserId { get; set; }
        public string Summary { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
