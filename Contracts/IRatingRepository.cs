using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

/// <summary>
/// Contract for rating persistence operations.
/// Follows ISP: only rating-related operations.
/// </summary>
public interface IRatingRepository
{
    IReadOnlyList<Rating> GetAll();
    IReadOnlyList<Rating> GetByMember(int accountId);
    Rating? GetByMemberAndBook(int accountId, int isbn);
    void AddOrUpdate(Rating rating);
}
