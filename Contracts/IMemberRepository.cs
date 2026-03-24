using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

/// <summary>
/// Contract for member persistence operations.
/// Follows ISP: only member-related operations.
/// </summary>
public interface IMemberRepository
{
    IReadOnlyList<Member> GetAll();
    Member? GetById(int accountId);
    Member? GetByName(string name);
    void Add(Member member);
    void LoadFromFile(string filePath, IRatingRepository ratingRepo, IBookRepository bookRepo);
}
