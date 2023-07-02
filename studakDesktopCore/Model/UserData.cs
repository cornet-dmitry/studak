using System;

namespace studakDesktopCore.Model;

public class UserData
{
    public int Id { get; set; }
    public string? UserLogin { get; set; }
    public string? Role { get; set; }
    public string? Surname { get; set; }
    public string? Name { get; set; }
    public string? Patronymic { get; set; }
    public DateTime? DateBirth { get; set; } 
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Messenger { get; set; }
    public int? Kpi { get; set; }
}
