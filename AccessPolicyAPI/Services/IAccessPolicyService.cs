using System.Security.Claims;

namespace AccessPolicyAPI.Services;

public interface IAccessPolicyService
{
    bool AccessCheck(ClaimsPrincipal user);
}
