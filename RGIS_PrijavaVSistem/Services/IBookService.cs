public interface IBookService
{
    Book AddBook(string title, string author, string genre, int pages);
    IEnumerable<Book> Search(string query);
    IEnumerable<Book> FilterByStatus(BookStatus status);
    IEnumerable<Book> FilterByGenre(string genre);
    void RateBook(Guid id, int rating);
}
