namespace DevSpaceShared.Responses;
public class DockerResponse<T>
{
    public DockerError Error = DockerError.None;
    public T Data;
}
public enum DockerError
{
    None, NotInstalled, Failed
}