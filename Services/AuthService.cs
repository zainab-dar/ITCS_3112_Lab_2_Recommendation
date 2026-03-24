using ITCS_3112_Lab_2_Recommendation.Contracts;
using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Services;

/// <summary>
/// Manages login/logout session state.
/// Depends on IMemberRepository abstraction (DIP).
/// Single Responsibility: only tracks who is logged in.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IMemberRepository _memberRepo;

    public bool IsLoggedIn => CurrentMember != null;
    public Member? CurrentMember { get; private set; }

    public AuthService(IMemberRepository memberRepo)
    {
        _memberRepo = memberRepo;
    }

    /// <summary>
    /// Attempts to log in by member name. Returns true on success.
    /// </summary>
    public bool Login(string name)
    {
        var member = _memberRepo.GetByName(name);
        if (member == null) return false;

        CurrentMember = member;
        return true;
    }

    public void Logout()
    {
        CurrentMember = null;
    }
}
