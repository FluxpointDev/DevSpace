namespace DevSpaceWeb.Data.Users;

public class UserDisabled
{
    public UserDisabledState State { get; set; }
    public string Reason { get; set; }
    public DateTime DisabledAt { get; set; }
}
public enum UserDisabledState
{
    User, TeamManaged, Instance
}
