using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Services;

/// <summary>
/// Generates personalized book recommendations using dot-product similarity.
///
/// Algorithm:
///   1. Build a rating vector for each member over the full book catalogue.
///   2. Compute the dot product of the target member's vector with every other member's vector.
///   3. Sort other members by descending similarity score.
///   4. Walk the most-similar member's rated books (highest rating first) and recommend
///      books the target member has NOT yet read (rating == 0 / NotRead).
/// </summary>
public class RecommendationService : IRecommendationService
{
    private readonly IMemberRepository _memberRepo;
    private readonly IBookRepository   _bookRepo;
    private readonly IRatingRepository _ratingRepo;

    public RecommendationService(
        IMemberRepository memberRepo,
        IBookRepository bookRepo,
        IRatingRepository ratingRepo)
    {
        _memberRepo  = memberRepo;
        _bookRepo    = bookRepo;
        _ratingRepo  = ratingRepo;
    }

    public IReadOnlyList<Book> GetRecommendations(Member member, int count = 5)
    {
        var allBooks   = _bookRepo.GetAll();
        var allMembers = _memberRepo.GetAll()
                                   .Where(m => m.AccountId != member.AccountId)
                                   .ToList();

        if (allBooks.Count == 0 || allMembers.Count == 0)
            return Array.Empty<Book>();

        // Build rating vector for the target member
        int[] targetVector = BuildVector(member, allBooks);

        // Compute similarity with every other member
        var similarities = allMembers
            .Select(other => new
            {
                Member     = other,
                Vector     = BuildVector(other, allBooks),
                Similarity = 0  // placeholder
            })
            .Select(x => new
            {
                x.Member,
                x.Vector,
                Similarity = DotProduct(targetVector, x.Vector)
            })
            .OrderByDescending(x => x.Similarity)
            .ToList();

        // Collect books the target member has NOT rated (value == 0)
        var unratedByTarget = new HashSet<int>(
            allBooks.Where(b => GetRatingValue(member, b) == 0)
                    .Select(b => b.ISBN));

        // Walk most-similar members and collect recommended books
        var recommendations = new List<Book>();
        var seen = new HashSet<int>();

        foreach (var sim in similarities)
        {
            if (recommendations.Count >= count) break;

            // Get this neighbour's positively-rated books they haven't read, sorted desc
            var candidateBooks = allBooks
                .Where(b => unratedByTarget.Contains(b.ISBN))
                .Where(b => GetRatingValue(sim.Member, b) > 0)
                .OrderByDescending(b => GetRatingValue(sim.Member, b));

            foreach (var book in candidateBooks)
            {
                if (recommendations.Count >= count) break;
                if (seen.Add(book.ISBN))
                    recommendations.Add(book);
            }
        }

        // If still short, fall back to highest-average unrated books
        if (recommendations.Count < count)
        {
            var avgRatings = allBooks
                .Where(b => unratedByTarget.Contains(b.ISBN) && !seen.Contains(b.ISBN))
                .Select(b => new
                {
                    Book = b,
                    Avg  = allMembers.Average(m => GetRatingValue(m, b))
                })
                .OrderByDescending(x => x.Avg);

            foreach (var item in avgRatings)
            {
                if (recommendations.Count >= count) break;
                recommendations.Add(item.Book);
            }
        }

        return recommendations.AsReadOnly();
    }

    // --- Helpers ---

    private int[] BuildVector(Member member, IReadOnlyList<Book> books)
    {
        return books.Select(b => GetRatingValue(member, b)).ToArray();
    }

    private int GetRatingValue(Member member, Book book)
    {
        var rating = _ratingRepo.GetByMemberAndBook(member.AccountId, book.ISBN);
        return rating == null ? 0 : (int)rating.Value;
    }

    private static int DotProduct(int[] a, int[] b)
    {
        int sum = 0;
        int len = Math.Min(a.Length, b.Length);
        for (int i = 0; i < len; i++)
            sum += a[i] * b[i];
        return sum;
    }
}
