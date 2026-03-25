using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Repositories;

/// <summary>
/// In-memory repository for books.
/// Reads books.txt exactly once; each line format: Author,Title,Year
/// </summary>
public class BookRepository : IBookRepository
{
    private readonly List<Book> _books = new();

    public IReadOnlyList<Book> GetAll() => _books.AsReadOnly();

    public Book? GetById(int isbn) =>
        _books.FirstOrDefault(b => b.ISBN == isbn);

    public void Add(Book book)
    {
        if (_books.Any(b => b.ISBN == book.ISBN))
            throw new InvalidOperationException($"Book with ISBN {book.ISBN} already exists.");
        _books.Add(book);
    }

    /// <summary>
    /// Loads books from a CSV file. Expected format per line:
    ///   Author,Title,Year
    /// Books are assigned sequential ISBNs starting at 1001.
    /// The file is read only once.
    /// </summary>
    public void LoadFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Books file not found: {path}");

        var lines = File.ReadAllLines(path);

        int isbn = 1;

        foreach (var line in lines)
        {
            var parts = line.Split(',');

            if (parts.Length < 3) continue;

            string author = parts[0].Trim();
            string title = parts[1].Trim();
            string year = parts[2].Trim();

            var book = new Book(author, title, year)
            {
                ISBN = isbn++
            };
            _books.Add(book); // ✅ FIXED
        }
    }
}
