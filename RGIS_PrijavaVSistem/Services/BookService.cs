using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookTracker.Domain
{
    public interface IBookService
    {
        // US1 Dodajanje nove knjige
        Book AddBook(Guid ownerId, string title, string author, string genre, int pages);

        // US2 Status
        void SetStatus(Guid bookId, BookStatus status);

        // US3 Zapiski
        void SetNotes(Guid bookId, string notes);

        // US4 Citat
        BookQuote AddQuote(Guid bookId, string text);
        IEnumerable<BookQuote> GetQuotes(Guid bookId);

        // US5 Naslovnica (fake upload)
        void UploadCover(Guid bookId, string fileName, long fileSizeBytes);

        // US6 Iskanje
        IEnumerable<Book> Search(Guid ownerId, string query);

        // US7 Urejanje / brisanje
        void UpdateBook(Guid bookId, string title, string author, string genre, int pages);
        void DeleteBook(Guid bookId, bool confirmed);

        // US8 Filtriranje
        IEnumerable<Book> FilterByStatus(Guid ownerId, BookStatus status);
        IEnumerable<Book> FilterByGenre(Guid ownerId, string genre);

        // US9 Ocena
        void RateBook(Guid bookId, int rating);

        // US10 – izvoz CSV
        string ExportBooksToCsv(Guid ownerId);
    }

    public class BookService : IBookService
    {
        private readonly InMemoryDataStore _db;

        public BookService(InMemoryDataStore db)
        {
            _db = db;
        }

        public Book AddBook(Guid ownerId, string title, string author, string genre, int pages)
        {
            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(genre))
                throw new ArgumentException("Vsa polja (naslov, avtor, žanr) so obvezna.");

            if (title.Length > 200)
                throw new ArgumentException("Naslov je predolg (max 200 znakov).");

            if (pages <= 0)
                throw new ArgumentException("Število strani mora biti > 0.");

            var book = new Book
            {
                OwnerId = ownerId,
                Title = title,
                Author = author,
                Genre = genre,
                Pages = pages
            };

            _db.Books.Add(book);
            return book;
        }

        public void SetStatus(Guid bookId, BookStatus status)
        {
            var book = GetBookOrThrow(bookId);
            book.Status = status;

            if (status == BookStatus.Prebrana && book.FinishedAt == null)
                book.FinishedAt = DateTime.UtcNow;
        }

        public void SetNotes(Guid bookId, string notes)
        {
            if (notes.Length > 2000)
                throw new ArgumentException("Zapiski so predolgi (max 2000 znakov).");

            var book = GetBookOrThrow(bookId);
            book.Notes = notes;
        }

        public BookQuote AddQuote(Guid bookId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Citat ne sme biti prazen.");

            var book = GetBookOrThrow(bookId);

            var quote = new BookQuote
            {
                BookId = book.Id,
                Text = text
            };

            _db.Quotes.Add(quote);
            return quote;
        }

        public IEnumerable<BookQuote> GetQuotes(Guid bookId) =>
            _db.Quotes.Where(q => q.BookId == bookId);

        public void UploadCover(Guid bookId, string fileName, long fileSizeBytes)
        {
            var book = GetBookOrThrow(bookId);

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                throw new ArgumentException("Dovoljene so samo .jpg ali .png slike.");

            const long maxBytes = 10 * 1024 * 1024; // 10 MB
            if (fileSizeBytes > maxBytes)
                throw new ArgumentException("Slika je prevelika (max 10 MB).");

            // tukaj bi dejansko shranili datoteko, mi pa samo zapomnimo pot
            book.CoverPath = "/covers/" + fileName;
        }

        public IEnumerable<Book> Search(Guid ownerId, string query)
        {
            var books = _db.Books.Where(b => b.OwnerId == ownerId);

            if (string.IsNullOrWhiteSpace(query))
                return books;

            query = query.ToLowerInvariant();
            return books.Where(b =>
                b.Title.ToLowerInvariant().Contains(query) ||
                b.Author.ToLowerInvariant().Contains(query));
        }

        public void UpdateBook(Guid bookId, string title, string author, string genre, int pages)
        {
            var book = GetBookOrThrow(bookId);
            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(author) ||
                string.IsNullOrWhiteSpace(genre))
                throw new ArgumentException("Vsa polja so obvezna.");

            book.Title = title;
            book.Author = author;
            book.Genre = genre;
            book.Pages = pages;
        }

        public void DeleteBook(Guid bookId, bool confirmed)
        {
            if (!confirmed)
                throw new InvalidOperationException("Brisanje mora biti potrjeno.");

            var book = GetBookOrThrow(bookId);
            _db.Books.Remove(book);
        }

        public IEnumerable<Book> FilterByStatus(Guid ownerId, BookStatus status) =>
            _db.Books.Where(b => b.OwnerId == ownerId && b.Status == status);

        public IEnumerable<Book> FilterByGenre(Guid ownerId, string genre) =>
            _db.Books.Where(b => b.OwnerId == ownerId &&
                                 string.Equals(b.Genre, genre, StringComparison.OrdinalIgnoreCase));

        public void RateBook(Guid bookId, int rating)
        {
            if (rating < 1 || rating > 10)
                throw new ArgumentOutOfRangeException(nameof(rating), "Ocena mora biti med 1 in 10.");

            var book = GetBookOrThrow(bookId);
            book.Rating = rating;
        }

        public string ExportBooksToCsv(Guid ownerId)
        {
            var books = _db.Books.Where(b => b.OwnerId == ownerId).ToList();
            var sb = new StringBuilder();
            sb.AppendLine("Title,Author,Genre,Pages,Status,Rating");

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title},{b.Author},{b.Genre},{b.Pages},{b.Status},{b.Rating}");
            }

            return sb.ToString();
        }

        private Book GetBookOrThrow(Guid bookId)
        {
            var book = _db.Books.SingleOrDefault(b => b.Id == bookId);
            if (book == null)
                throw new KeyNotFoundException("Knjiga ne obstaja.");
            return book;
        }
    }
}
