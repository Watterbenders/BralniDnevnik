using System;
using System.Collections.Generic;
using System.Linq;

namespace BookTracker.Domain
{
    public interface IRecommendationService
    {
        // US19 – glede na cilje (enostavno: preferiramo krajše knjige)
        IEnumerable<Book> RecommendByLengthAndGenre(Guid userId, int maxPages, string? preferredGenre);

        // US26 – priporočila med uporabniki
        Recommendation SendRecommendation(Guid fromUserId, Guid toUserId, Guid bookId, string? note);
        IEnumerable<Recommendation> GetReceivedRecommendations(Guid userId);
    }

    public interface IChallengeService
    {
        Challenge CreateChallenge(Guid userId, string description);   // US16
        void CompleteChallenge(Guid challengeId);
    }

    public interface ILeaderboardService
    {
        // US28 – top bralci
        IEnumerable<(User user, int points)> GetTopReaders(LeaderboardPeriod period);
    }

    public interface ICommentService
    {
        Comment AddComment(Guid bookId, Guid authorId, string text);  // US27
        void DeleteComment(Guid commentId, Guid requesterId);
        IEnumerable<Comment> GetComments(Guid bookId);
    }

    public class RecommendationService : IRecommendationService
    {
        private readonly InMemoryDataStore _db;

        public RecommendationService(InMemoryDataStore db)
        {
            _db = db;
        }

        public IEnumerable<Book> RecommendByLengthAndGenre(Guid userId, int maxPages, string? preferredGenre)
        {
            // zelo simpl: priporočamo knjige drugih uporabnikov
            var books = _db.Books.Where(b => b.OwnerId != userId);

            if (!string.IsNullOrWhiteSpace(preferredGenre))
            {
                books = books.Where(b =>
                    string.Equals(b.Genre, preferredGenre, StringComparison.OrdinalIgnoreCase));
            }

            return books
                .Where(b => b.Pages <= maxPages)
                .OrderBy(b => b.Pages)
                .Take(10);
        }

        public Recommendation SendRecommendation(Guid fromUserId, Guid toUserId, Guid bookId, string? note)
        {
            var rec = new Recommendation
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
                BookId = bookId,
                Note = note
            };

            _db.Recommendations.Add(rec);
            return rec;
        }

        public IEnumerable<Recommendation> GetReceivedRecommendations(Guid userId) =>
            _db.Recommendations.Where(r => r.ToUserId == userId);
    }

    public class ChallengeService : IChallengeService
    {
        private readonly InMemoryDataStore _db;

        public ChallengeService(InMemoryDataStore db)
        {
            _db = db;
        }

        public Challenge CreateChallenge(Guid userId, string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Opis izziva je obvezen.");

            var c = new Challenge
            {
                UserId = userId,
                Description = description,
                Completed = false
            };
            _db.Challenges.Add(c);
            return c;
        }

        public void CompleteChallenge(Guid challengeId)
        {
            var c = _db.Challenges.SingleOrDefault(x => x.Id == challengeId)
                    ?? throw new KeyNotFoundException("Izziv ne obstaja.");
            c.Completed = true;
        }
    }

    public class LeaderboardService : ILeaderboardService
    {
        private readonly InMemoryDataStore _db;

        public LeaderboardService(InMemoryDataStore db)
        {
            _db = db;
        }

        public IEnumerable<(User user, int points)> GetTopReaders(LeaderboardPeriod period)
        {
            DateTime from = period switch
            {
                LeaderboardPeriod.Day => DateTime.UtcNow.Date,
                LeaderboardPeriod.Week => DateTime.UtcNow.Date.AddDays(-7),
                LeaderboardPeriod.Month => DateTime.UtcNow.Date.AddMonths(-1),
                LeaderboardPeriod.Year => DateTime.UtcNow.Date.AddYears(-1),
                _ => DateTime.UtcNow.Date
            };

            var books = _db.Books
                .Where(b => b.FinishedAt.HasValue && b.FinishedAt.Value >= from);

            var grouped = books
                .GroupBy(b => b.OwnerId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Points = g.Sum(b => b.Pages) // npr. štejemo strani kot točke
                })
                .OrderByDescending(x => x.Points)
                .Take(10)
                .ToList();

            var result = new List<(User user, int points)>();

            foreach (var g in grouped)
            {
                var user = _db.Users.SingleOrDefault(u => u.Id == g.UserId);
                if (user != null)
                    result.Add((user, g.Points));
            }

            return result;
        }
    }

    public class CommentService : ICommentService
    {
        private readonly InMemoryDataStore _db;

        public CommentService(InMemoryDataStore db)
        {
            _db = db;
        }

        public Comment AddComment(Guid bookId, Guid authorId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Komentar ne sme biti prazen.");

            var c = new Comment
            {
                BookId = bookId,
                AuthorId = authorId,
                Text = text
            };
            _db.Comments.Add(c);
            return c;
        }

        public void DeleteComment(Guid commentId, Guid requesterId)
        {
            var c = _db.Comments.SingleOrDefault(x => x.Id == commentId)
                    ?? throw new KeyNotFoundException("Komentar ne obstaja.");

            // simple: lahko briše samo avtor
            if (c.AuthorId != requesterId)
                throw new InvalidOperationException("Komentar lahko izbriše samo avtor.");

            _db.Comments.Remove(c);
        }

        public IEnumerable<Comment> GetComments(Guid bookId) =>
            _db.Comments.Where(c => c.BookId == bookId).OrderByDescending(c => c.CreatedAt);
    }
}
