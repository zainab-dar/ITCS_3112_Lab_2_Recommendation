using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Repositories;

/// <summary>
/// In-memory repository for ratings.
/// Uses a composite key (accountId, isbn) for fast lookup.
/// </summary>
public class RatingRepository : IRatingRepository
{
    // Key: (accountId, isbn)
    private readonly Dictionary<(int, int), Rating> _ratings = new();

    public IReadOnlyList<Rating> GetAll() =>
        _ratings.Values.ToList().AsReadOnly();

    public IReadOnlyList<Rating> GetByMember(int accountId) =>
        _ratings.Values
                .Where(r => r.Member.AccountId == accountId)
                .ToList()
                .AsReadOnly();

    public Rating? GetByMemberAndBook(int accountId, int isbn) =>
        _ratings.TryGetValue((accountId, isbn), out var rating) ? rating : null;

    /// <summary>
    /// Adds a new rating or updates an existing one for the same member/book pair.
    /// </summary>
    public void AddOrUpdate(Rating rating)
    {
        var key = (rating.Member.AccountId, rating.Book.ISBN);
        _ratings[key] = rating;
    }
}
