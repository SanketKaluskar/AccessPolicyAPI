using AccessPolicyAPI.Authorization;
using AccessPolicyAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace AccessPolicyAPI.Tests;

[TestClass]
public class AuthorizationHandlersTests
{
    [TestMethod]
    public async Task Successful_WhenAccessPolicyServiceGrantsAccess()
    {
        var context = new AuthorizationHandlerContext([new ProjectAndRoleRequirement()], new ClaimsPrincipal(), null);

        var apsMock = new Mock<IAccessPolicyService>();
        apsMock.Setup(aps => aps.AccessCheck(It.IsAny<ClaimsPrincipal>())).Returns(true); // <-- Simulate access granted

        var loggerMock = new Mock<ILogger<ProjectAndRoleHandler>>();
        var handler = new ProjectAndRoleHandler(apsMock.Object, loggerMock.Object);

        await handler.HandleAsync(context);
        Assert.IsTrue(context.HasSucceeded);
    }

    [TestMethod]
    public async Task Unsuccessful_WhenAccessPolicyServiceDeniesAccess()
    {
        var context = new AuthorizationHandlerContext([new ProjectAndRoleRequirement()], new ClaimsPrincipal(), null);

        var apsMock = new Mock<IAccessPolicyService>();
        apsMock.Setup(aps => aps.AccessCheck(It.IsAny<ClaimsPrincipal>())).Returns(false); // <-- Simulate access denied

        var loggerMock = new Mock<ILogger<ProjectAndRoleHandler>>();
        var handler = new ProjectAndRoleHandler(apsMock.Object, loggerMock.Object);

        await handler.HandleAsync(context);
        Assert.IsFalse(context.HasSucceeded);
    }
}
