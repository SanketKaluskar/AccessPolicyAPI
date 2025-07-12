using AccessPolicyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AccessPolicyAPI.Authorization;

public class ProjectAndRoleHandler : AuthorizationHandler<ProjectAndRoleRequirement>
{
    private readonly IAccessPolicyService _accessService;
    private readonly ILogger<ProjectAndRoleHandler> _logger;
    public ProjectAndRoleHandler(
        IAccessPolicyService accessService, 
        ILogger<ProjectAndRoleHandler> logger)
    {
        _accessService = accessService;
        _logger = logger;
    }
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        ProjectAndRoleRequirement requirement)
    {
        ClaimsPrincipal user = context.User;
        bool granted = _accessService.AccessCheck(user);

        string subject = user.FindFirst(Claims.Subject)?.Value ?? "unknown";
        string project = user.FindFirst(Claims.Project)?.Value ?? "none";
        string scope = user.FindFirst(Claims.Scope)?.Value ?? "none";
        IEnumerable<string> roles = user.FindAll(Claims.Role).Select(c => c.Value);
        _logger.LogInformation($"Access Check | Subject: {subject} | Project: {project} | Roles: {string.Join(",", roles)} | Scope: {scope} | Decision: {(granted ? "Granted" : "Denied")}");

        if (granted)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}

