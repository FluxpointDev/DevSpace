namespace Docker.DotNet;

internal interface IRequestContent
{
    HttpContent GetContent();
}