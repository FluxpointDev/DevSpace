namespace DevSpaceShared.Responses;
public class FirewallResponse
{
    public bool IsEnabled;
    public bool IsLoggingEnabled;
    public string LoggingMode;
    public FirewallDefaultResponse Default = new FirewallDefaultResponse();
    public List<FirewallRuleResponse> Rules = new List<FirewallRuleResponse>();
}
public class FirewallDefaultResponse
{
    public string Incoming;
    public string Outgoing;
    public string Routed;
}
public class FirewallRuleResponse
{
    public string To;
    public string Action;
    public string From;
    public string Comment;
}
