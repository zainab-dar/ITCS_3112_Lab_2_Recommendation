namespace ITCS_3112_Lab_2_Recommendation.Domain;

/// <summary>
/// Valid rating values with their emotional meanings.
/// </summary>
public enum RatingValue
{
    HatedIt = -5,
    DidntLike = -3,
    NotRead = 0,
    Neutral = 1,
    Liked = 3,
    ReallyLiked = 5
}

/// <summary>
/// Represents a single rating given by a Member for a Book.
/// </summary>
public class Rating
{
    public Book Book { get; }
    public Member Member { get; }
    public RatingValue Value { get; set; }

    public Rating(Book book, Member member, RatingValue value)
    {
        Book = book;
        Member = member;
        Value = value;
    }

    /// <summary>
    /// Returns the emoji label for the current rating value.
    /// </summary>
    public string GetLabel() => Value switch
    {
        RatingValue.HatedIt     => "😡 Hated it!",
        RatingValue.DidntLike   => "🙁 Didn't like it",
        RatingValue.NotRead     => "🤷 Haven't read it",
        RatingValue.Neutral     => "😐 Ok – neither hot nor cold",
        RatingValue.Liked       => "🙂 Liked it!",
        RatingValue.ReallyLiked => "🤩 Really liked it!",
        _ => "Unknown"
    };

    public override string ToString() =>
        $"{Book.Title} — {(int)Value} ({GetLabel()})";
}
