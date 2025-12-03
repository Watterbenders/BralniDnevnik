using System;
using System.Collections.Generic;
using System.Linq;

namespace BookTracker.Domain
{
    public interface IReadingProgressService
    {
        double UpdateProgress(Guid bookId, int pagesRead); // vrne % napredka – US12
    }

    public class ReadingProgressService : IReadingProgressService
    {
        private readonly InMemoryDataStore _db;

        public ReadingProgressService(InMemoryDataStore db)
        {
            _db = db;
        }

        public double UpdateProgress(Guid bookId, int pagesRead)
        {
            var book = _db.Books.SingleOrDefault(b => b.Id == bookId)
                       ?? throw new KeyNotFoundException("Knjiga ne obstaja.");

            if (pagesRead < 0 || pagesRead > book.Pages)
                throw new ArgumentOutOfRangeException(nameof(pagesRead), "Neveljavno število prebranih strani.");

            var progress = _db.Progresses.SingleOrDefault(p => p.BookId == bookId);
            if (progress == null)
            {
                progress = new ReadingProgress
                {
                    BookId = bookId,
                    PagesRead = pagesRead
                };
                _db.Progresses.Add(progress);
            }
            else
            {
                progress.PagesRead = pagesRead;
            }

            double percent = book.Pages == 0 ? 0 : (double)progress.PagesRead / book.Pages * 100;
            return percent;
        }
    }
}
