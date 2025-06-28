using System.Net.Http.Headers;

namespace DevSpaceWeb.Apps.Runtime;

public class ResponseData
{
    public bool IsSuccess;
    public int StatusCode;
    public long ContentLength;
    public string ContentType;
    public HttpHeaders Headers;
}
