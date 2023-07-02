namespace studakDesktopCore.Model;

public class OrganizationData
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string ShortName { get; set; } = null!;
    public int Level { get; set; }
    public string? Address { get; set; }
}