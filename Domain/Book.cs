namespace ITCS_3112_Lab_2_Recommendation.Domain;

/// <summary>
/// Represents a book in the system.
/// ISBN is auto-generated as a unique identifier.
/// </summary>
public class Book
{
    private static int _nextIsbn = 1001;

    public int ISBN { get; }
    public string Author { get; set; }
    public string Title { get; set; }
    public string Year { get; set; }

    /// <summary>
    /// Creates a new Book with an auto-generated ISBN.
    /// </summary>
    public Book(string author, string title, string year)
    {
        ISBN = _nextIsbn++;
        Author = author;
        Title = title;
        Year = year;
    }

    /// <summary>
    /// Creates a Book with a specific ISBN (used when loading from file).
    /// </summary>
    public Book(int isbn, string author, string title, string year)
    {
        if (isbn >= _nextIsbn) _nextIsbn = isbn + 1;
        ISBN = isbn;
        Author = author;
        Title = title;
        Year = year;
    }

    public override string ToString() => $"[{ISBN}] \"{Title}\" by {Author} ({Year})";
}
