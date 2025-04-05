namespace DevSpaceShared.Data;

public class SecurityData
{
    public bool? IsComplete { get; set; }
    public string? Logs { get; set; }
}
public class SecurityReport
{
    public SecurityResult[]? Results { get; set; }
}
public class SecurityResult
{
    public string? CurrentSeverity { get; set; }
    public string? Target { get; set; }
    public string? Type { get; set; }
    public SecurityVulnerability[]? Vulnerabilities { get; set; }
}
public class SecurityVulnerability
{
    public string? VulnerabilityID { get; set; }
    public string? PkgName { get; set; }
    public string? InstalledVersion { get; set; }
    public string? FixedVersion { get; set; }
    public string? Status { get; set; }
    public SecuritySource? DataSource { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Severity { get; set; }
    public DateTime? PublishedDate { get; set; }
}
public class SecuritySource
{
    public string? Name { get; set; }
    public string? URL { get; set; }

    public string GetUrl(SecurityVulnerability info)
    {
        if (!string.IsNullOrEmpty(URL) && URL.StartsWith("https://github.com/advisories"))
            return URL + "+" + info.VulnerabilityID;

        return URL;
    }
}