namespace AccessPolicyAPI.Models;

public class ProjectPolicyOptions
{
    public string RequiredProject { get; set; } = string.Empty;
    public List<string> AllowedRoles { get; set; } = [];
    public string RequiredScope { get; set; } = string.Empty;
}

