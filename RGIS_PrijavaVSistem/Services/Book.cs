using System;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Pages { get; set; }
    public int? Rating { get; set; }   // 1–10
    public BookStatus Status { get; set; } = BookStatus.NaCakanju;
    public DateTime FinishedAt { get; internal set; }
}
