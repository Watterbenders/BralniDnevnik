using System;
using System.Collections.Generic;
using System.Linq;

namespace BookTracker.Domain
{
    public interface IImportExportService
    {
        string ExportCsv(Guid userId);                    // US10
        string ExportPdfReport(Guid userId);              // US10
        void ImportCsv(Guid userId, string csvContent);   // US10
    }

    public class ImportExportService : IImportExportService
    {
        private readonly InMemoryDataStore _db;
        private readonly IBookService _books;

        public ImportExportService(InMemoryDataStore db, IBookService books)
        {
            _db = db;
            _books = books;
        }

        public string ExportCsv(Guid userId)
        {
            return _books.ExportBooksToCsv(userId);
        }

        public string ExportPdfReport(Guid userId)
        {
            // za potrebe naloge: "PDF" bo navaden tekst, ki ga lahko
            // pozneje pretvoriš v pravi PDF
            var books = _db.Books.Where(b => b.OwnerId == userId).ToList();
            var lines = new List<string>
            {
                "Poročilo o branju",
                $"Število knjig: {books.Count}",
                ""
            };

            foreach (var b in books)
            {
                lines.Add($"{b.Title} – {b.Author} ({b.Pages} strani, status: {b.Status})");
            }

            return string.Join(Environment.NewLine, lines);
        }

        public void ImportCsv(Guid userId, string csvContent)
        {
            var lines = csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            // preskočimo header
            foreach (var line in lines.Skip(1))
            {
                var parts = line.Trim().Split(',');
                if (parts.Length < 4) continue;

                var title = parts[0];
                var author = parts[1];
                var genre = parts[2];
                if (!int.TryParse(parts[3], out int pages)) continue;

                _books.AddBook(userId, title, author, genre, pages);
            }
        }
    }
}
