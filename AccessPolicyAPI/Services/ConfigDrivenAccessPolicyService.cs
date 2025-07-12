using AccessPolicyAPI.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace AccessPolicyAPI.Services;

public class ConfigDrivenAccessPolicyService : IAccessPolicyService
{
    private readonly ProjectPolicyOptions _options;
    public ConfigDrivenAccessPolicyService(IOptions<ProjectPolicyOptions> options)
    {
        _options = options.Value;
    }

    public bool AccessCheck(ClaimsPrincipal user)
    {
        string project = user.FindFirst(Claims.Project)?.Value ?? string.Empty;
        var roles = new HashSet<string>(user.FindAll(Claims.Role).Select(c => c.Value), StringComparer.OrdinalIgnoreCase);
        var scopes = new HashSet<string>((user.FindFirst(Claims.Scope)?.Value ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries), StringComparer.OrdinalIgnoreCase);

        bool projectMatch = string.Equals(project, _options.RequiredProject, StringComparison.OrdinalIgnoreCase);
        bool roleMatch = _options.AllowedRoles.Any(role => user.IsInRole(role) || roles.Contains(role));

        // This policy should pertain only to the user principal (subject), not the client app.
        // Checking client app's attributes (consented scope) along with subject's roles and claims
        // together in one location is not great, and the scope check should be refactored out elsewhere.
        bool scopeMatch = scopes.Contains(_options.RequiredScope);

        return projectMatch && roleMatch && scopeMatch;
    }
}

