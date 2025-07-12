using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessPolicyAPI.Controllers;

[ApiController]
[Route("api/secure")]
public class SecureController : ControllerBase
{
    [HttpGet("files")]
    [Authorize(Policy = "JuhuAdminPolicy")]
    public async Task<IActionResult> GetSecureFiles()
    {
        string project = User.FindFirst(Claims.Project)?.Value ?? string.Empty;
        var roles = User.FindAll(Claims.Role).Select(role => role.Value);
        var projectFiles = await Task.Run(() => 
            new List<(string FileName, string Project)>
            {
                ("specs-juhu.pdf", "Juhu"),
                ("budget-juhu.xlsx", "Juhu"),
                ("roadmap-versova.pdf", "Versova"),
                ("notes-juhu.txt", "Juhu")
            }
            .Where(item => item.Project == project)
            .Select(item => item.FileName));

        return Ok(new
        {
            Project = project,
            Roles = roles,
            AuthorizedFiles = projectFiles
        });
    }
}
