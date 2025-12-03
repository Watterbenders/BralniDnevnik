using System;
using System.Collections.Generic;
using System.Linq;

namespace BookTracker.Domain
{
    public class MonthlyStat
    {
        public int Year { get; set; }
        public int Month { get; set; } // 1–12
        public int Books { get; set; }
        public int Pages { get; set; }
    }

    public class GenreStat
    {
        public string Genre { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class AuthorStat
    {
        public string Author { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public interface IStatsService
    {
        // US14 + 21 – podatki za grafe
        IEnumerable<MonthlyStat> GetMonthlyStats(Guid userId, int year);

        // US20 – primerjava let
        (int booksYear1, int booksYear2) CompareYears(Guid userId, int year1, int year2);

        // US22 – statistika knjig
        int GetTotalPages(Guid userId);
        IEnumerable<GenreStat> GetGenreBreakdown(Guid userId);

        // US23 – najbolj bran žanr/avtor
        IEnumerable<GenreStat> GetTopGenres(Guid userId, int top);
        IEnumerable<AuthorStat> GetTopAuthors(Guid userId, int top);

        // US24 – bralni profil
        ReadingProfile GenerateOrUpdateProfile(Guid userId);
        ReadingProfile? GetProfile(Guid userId);
    }

    public class StatsService : IStatsService
    {
        private readonly InMemoryDataStore _db;

        public StatsService(InMemoryDataStore db)
        {
            _db = db;
        }

        public IEnumerable<MonthlyStat> GetMonthlyStats(Guid userId, int year)
        {
            var books = _db.Books.Where(b =>
                b.OwnerId == userId &&
                b.FinishedAt.HasValue &&
                b.FinishedAt.Value.Year == year);

            var stats = books
                .GroupBy(b => b.FinishedAt!.Value.Month)
                .Select(g => new MonthlyStat
                {
                    Year = year,
                    Month = g.Key,
                    Books = g.Count(),
                    Pages = g.Sum(b => b.Pages)
                })
                .ToList();

            // poskrbimo za vseh 12 mesecev
            for (int m = 1; m <= 12; m++)
            {
                if (!stats.Any(s => s.Month == m))
                {
                    stats.Add(new MonthlyStat { Year = year, Month = m, Books = 0, Pages = 0 });
                }
            }

            return stats.OrderBy(s => s.Month);
        }

        public (int booksYear1, int booksYear2) CompareYears(Guid userId, int year1, int year2)
        {
            int c1 = CountBooksInYear(userId, year1);
            int c2 = CountBooksInYear(userId, year2);
            return (c1, c2);
        }

        private int CountBooksInYear(Guid userId, int year)
        {
            return _db.Books.Count(b =>
                b.OwnerId == userId &&
                b.Status == BookStatus.Prebrana &&
                b.FinishedAt.HasValue &&
                b.FinishedAt.Value.Year == year);
        }

        public int GetTotalPages(Guid userId)
        {
            return _db.Books
                .Where(b => b.OwnerId == userId && b.Status == BookStatus.Prebrana)
                .Sum(b => b.Pages);
        }

        public IEnumerable<GenreStat> GetGenreBreakdown(Guid userId)
        {
            return _db.Books
                .Where(b => b.OwnerId == userId)
                .GroupBy(b => b.Genre)
                .Select(g => new GenreStat
                {
                    Genre = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(g => g.Count);
        }

        public IEnumerable<GenreStat> GetTopGenres(Guid userId, int top)
        {
            return GetGenreBreakdown(userId).Take(top);
        }

        public IEnumerable<AuthorStat> GetTopAuthors(Guid userId, int top)
        {
            return _db.Books
                .Where(b => b.OwnerId == userId)
                .GroupBy(b => b.Author)
                .Select(g => new AuthorStat
                {
                    Author = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(a => a.Count)
                .Take(top);
        }

        public ReadingProfile GenerateOrUpdateProfile(Guid userId)
        {
            var books = _db.Books.Where(b => b.OwnerId == userId).ToList();
            var topGenre = GetTopGenres(userId, 1).FirstOrDefault()?.Genre ?? "ni podatkov";
            var topAuthor = GetTopAuthors(userId, 1).FirstOrDefault()?.Author ?? "ni podatkov";
            var totalPages = GetTotalPages(userId);
            var totalBooks = books.Count;

            var summary =
                $"Prebral si {totalBooks} knjig s skupaj {totalPages} stranmi. " +
                $"Najpogostejši žanr: {topGenre}. " +
                $"Najpogostejši avtor: {topAuthor}.";

            var profile = _db.Profiles.SingleOrDefault(p => p.UserId == userId);
            if (profile == null)
            {
                profile = new ReadingProfile
                {
                    UserId = userId,
                    Summary = summary,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.Profiles.Add(profile);
            }
            else
            {
                profile.Summary = summary;
                profile.UpdatedAt = DateTime.UtcNow;
            }

            return profile;
        }

        public ReadingProfile? GetProfile(Guid userId) =>
            _db.Profiles.SingleOrDefault(p => p.UserId == userId);
    }
}
