using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

/// <summary>
/// Contract for generating book recommendations.
/// Decoupled from data layer; depends on abstractions (DIP).
/// </summary>
public interface IRecommendationService
{
    /// <summary>
    /// Returns up to <paramref name="count"/> recommended books for the given member,
    /// using dot-product similarity against all other members.
    /// Only books the member has NOT rated are recommended.
    /// </summary>
    IReadOnlyList<Book> GetRecommendations(Member member, int count = 5);
}
