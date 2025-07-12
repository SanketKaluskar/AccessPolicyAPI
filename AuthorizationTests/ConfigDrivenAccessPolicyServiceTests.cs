using AccessPolicyAPI.Models;
using AccessPolicyAPI.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Claims;

namespace AccessPolicyAPI.Tests;

[TestClass]
public class ConfigDrivenAccessPolicyServiceTests
{
    private ClaimsPrincipal CreateUser(string project, IEnumerable<string> roles, string scopes)
    {
        var claims = new List<Claim>
        {
            new Claim(Claims.Project, project),
            new Claim(Claims.Scope, scopes)
        };
        
        foreach (var role in roles)
        {
            claims.Add(new Claim(Claims.Role, role));
        }
        
        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }

    private ConfigDrivenAccessPolicyService CreateService()
    {
        var configured = new ProjectPolicyOptions
        {
            RequiredProject = "Abracadbra",
            AllowedRoles = ["Conjurer", "Assistant"],
            RequiredScope = "Lagomorph.From.Hat"
        };

        return new ConfigDrivenAccessPolicyService(Options.Create(configured));
    }

    [TestMethod]
    public void AccessCheck_Succeeds_WhenProjectRoleAndScopeMatch()
    {
        var user = CreateUser(
            project: "Abracadbra",
            roles: ["Conjurer"],
            scopes: "Lagomorph.From.Hat");

        var service = CreateService();
        Assert.IsTrue(service.AccessCheck(user));
    }

    [TestMethod]
    public void AccessCheck_Fails_WhenProjectDoesNotMatch()
    {
        var user = CreateUser(
            project: "Hocuspocus",
            roles: ["Conjurer"],
            scopes: "Lagomorph.From.Hat");

        var service = CreateService();
        Assert.IsFalse(service.AccessCheck(user));
    }

    [TestMethod]
    public void AccessCheck_Fails_WhenRoleIsMissing()
    {
        var user = CreateUser(
            project: "Abracadbra",
            roles: [],
            scopes: "Lagomorph.From.Hat");

        var service = CreateService();
        Assert.IsFalse(service.AccessCheck(user));
    }

    [TestMethod]
    public void AccessCheck_Fails_WhenScopeDoesNotMatch()
    {
        var user = CreateUser(
            project: "Abracadbra",
            roles: new[] { "Admin" },
            scopes: "Able.To.Vanish");

        var service = CreateService();
        Assert.IsFalse(service.AccessCheck(user));
    }
}

