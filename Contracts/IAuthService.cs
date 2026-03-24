using ITCS_3112_Lab_2_Recommendation.Domain;

namespace ITCS_3112_Lab_2_Recommendation.Contracts;

/// <summary>
/// Contract for login/logout session management.
/// </summary>
public interface IAuthService
{
    bool IsLoggedIn { get; }
    Member? CurrentMember { get; }
    bool Login(string name);
    void Logout();
}
