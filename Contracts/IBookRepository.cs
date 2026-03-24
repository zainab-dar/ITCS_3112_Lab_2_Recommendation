using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

/// <summary>
/// Contract for book persistence operations.
/// Follows ISP: only book-related operations.
/// </summary>
public interface IBookRepository
{
    IReadOnlyList<Book> GetAll();
    Book? GetById(int isbn);
    void Add(Book book);
    void LoadFromFile(string filePath);
}
