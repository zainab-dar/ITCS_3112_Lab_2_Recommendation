namespace ITCS_3112_Lab_2_Recommendation.Domain;

/// <summary>
/// Represents a member (user) of the book recommendation system.
/// Account ID is auto-generated as a unique identifier.
/// </summary>
public class Member
{
    private static int _nextAccountId = 1;

    public int AccountId { get; }
    public string Name { get; set; }

    /// <summary>
    /// Creates a new Member with an auto-generated Account ID.
    /// </summary>
    public Member(string name)
    {
        AccountId = _nextAccountId++;
        Name = name;
    }

    /// <summary>
    /// Creates a Member with a specific Account ID (used when loading from file).
    /// </summary>
    public Member(int accountId, string name)
    {
        if (accountId >= _nextAccountId) _nextAccountId = accountId + 1;
        AccountId = accountId;
        Name = name;
    }

    public override string ToString() => $"[{AccountId}] {Name}";
}
