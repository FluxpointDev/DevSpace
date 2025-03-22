namespace DevSpaceShared.Responses
{
    public class SocketResponse : SocketResponse<object> { }
    public class SocketResponse<T>
    {
        public bool IsSuccess;
        public ClientError Error;
        public DockerError DockerError;
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    public enum ClientError
    {
        None, Exception, Timeout, AuthError, CertError, JsonError
    }

    public enum DockerError
    {
        None, NotInstalled, Exception
    }
}
