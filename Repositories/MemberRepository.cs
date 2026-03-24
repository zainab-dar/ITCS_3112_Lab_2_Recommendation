using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Repositories;

/// <summary>
/// In-memory repository for members.
/// Reads ratings.txt exactly once; each line format:
///   MemberName,rating1,rating2,...,ratingN
/// Ratings correspond positionally to books in the BookRepository.
/// </summary>
public class MemberRepository : IMemberRepository
{
    private readonly List<Member> _members = new();

    public IReadOnlyList<Member> GetAll() => _members.AsReadOnly();

    public Member? GetById(int accountId) =>
        _members.FirstOrDefault(m => m.AccountId == accountId);

    public Member? GetByName(string name) =>
        _members.FirstOrDefault(m =>
            m.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void Add(Member member)
    {
        _members.Add(member);
    }

    /// <summary>
    /// Loads members and their ratings from a CSV file.
    /// Expected format per line: MemberName,rating1,rating2,...
    /// Ratings are matched positionally to books already loaded in bookRepo.
    /// The file is read only once.
    /// </summary>
    public void LoadFromFile(string filePath, IRatingRepository ratingRepo, IBookRepository bookRepo)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Ratings file not found: {filePath}");

        var books = bookRepo.GetAll();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split(',');
            if (parts.Length < 1)
            {
                Console.WriteLine($"  [WARN] Skipping malformed ratings line: {line}");
                continue;
            }

            string memberName = parts[0].Trim();
            var member = new Member(memberName);
            _members.Add(member);

            // Parse each rating value and match it to the corresponding book by position
            for (int i = 1; i < parts.Length; i++)
            {
                int bookIndex = i - 1;
                if (bookIndex >= books.Count) break;

                if (!int.TryParse(parts[i].Trim(), out int rawValue))
                {
                    Console.WriteLine($"  [WARN] Invalid rating value '{parts[i]}' for {memberName}, skipping.");
                    continue;
                }

                // Map raw int to enum; default to NotRead if unrecognized
                RatingValue rv = rawValue switch
                {
                    -5 => RatingValue.HatedIt,
                    -3 => RatingValue.DidntLike,
                     0 => RatingValue.NotRead,
                     1 => RatingValue.Neutral,
                     3 => RatingValue.Liked,
                     5 => RatingValue.ReallyLiked,
                    _  => RatingValue.NotRead
                };

                var rating = new Rating(books[bookIndex], member, rv);
                ratingRepo.AddOrUpdate(rating);
            }
        }
    }
}
