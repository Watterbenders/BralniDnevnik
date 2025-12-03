using System;
using System.Collections.Generic;
using System.Linq;

namespace BookTracker.Domain
{
    public interface IReadingGoalService
    {
        ReadingGoal SetGoal(Guid userId, int year, int targetBooks); // US11
        ReadingGoal ChangeGoal(Guid userId, int year, int newTargetBooks);
        (int completed, int target, double percent) GetYearProgress(Guid userId, int year); // US13
    }

    public class ReadingGoalService : IReadingGoalService
    {
        private readonly InMemoryDataStore _db;

        public ReadingGoalService(InMemoryDataStore db)
        {
            _db = db;
        }

        public ReadingGoal SetGoal(Guid userId, int year, int targetBooks)
        {
            var existing = _db.Goals.SingleOrDefault(g => g.UserId == userId && g.Year == year);
            if (existing != null)
                throw new InvalidOperationException("Cilj za to leto že obstaja.");

            var goal = new ReadingGoal
            {
                UserId = userId,
                Year = year,
                TargetBooks = targetBooks
            };
            _db.Goals.Add(goal);
            return goal;
        }

        public ReadingGoal ChangeGoal(Guid userId, int year, int newTargetBooks)
        {
            var goal = _db.Goals.SingleOrDefault(g => g.UserId == userId && g.Year == year)
                       ?? throw new KeyNotFoundException("Cilj ne obstaja.");

            goal.TargetBooks = newTargetBooks;
            return goal;
        }

        public (int completed, int target, double percent) GetYearProgress(Guid userId, int year)
        {
            var goal = _db.Goals.SingleOrDefault(g => g.UserId == userId && g.Year == year);
            if (goal == null) return (0, 0, 0);

            var completedBooks = _db.Books
                .Where(b => b.OwnerId == userId &&
                            b.Status == BookStatus.Prebrana &&
                            b.FinishedAt.HasValue &&
                            b.FinishedAt.Value.Year == year)
                .Count();

            double percent = goal.TargetBooks == 0
                ? 0
                : (double)completedBooks / goal.TargetBooks * 100;

            return (completedBooks, goal.TargetBooks, percent);
        }
    }
}
