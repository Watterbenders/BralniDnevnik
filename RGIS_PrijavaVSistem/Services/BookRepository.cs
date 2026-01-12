using System.Collections.Generic;
using System;
using System.Linq;
using RGIS_PrijavaVSistem.Models;

namespace RGIS_PrijavaVSistem.Services
{
    public static class BookRepository
    {
        private static readonly List<Book> _books = new();

        public static List<Book> GetAll() => _books;

        public static void Add(Book book)
        {
            _books.Add(book);
        }

        public static List<Book> Search(string query)
        {
            return _books
                .Where(b =>
                    b.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    b.Author.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}

