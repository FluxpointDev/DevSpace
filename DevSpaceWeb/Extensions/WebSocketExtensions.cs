using DevSpaceShared.Responses;
using DevSpaceShared.Services;
using DevSpaceShared.WebSocket;
using Radzen;

namespace DevSpaceWeb;

public static class WebSocketExtensions
{
    public static async Task<SocketResponse<T?>> RunJsonAsync<T>(this WebSocketClient client, NotificationService notification, IWebSocketTask json, Action<SocketResponse<T?>>? action = null, CancellationToken token = default) where T : class
    {
        SocketResponse<T?> Result = await client.RecieveJsonAsync<T>(json, token);
        if (Result.IsSuccess)
        {
            action?.Invoke(Result);
        }
        else
        {
            switch (Result.Error)
            {
                case ClientError.Timeout:
                    Result.Message = "Request timed out.";
                    break;
                case ClientError.JsonError:
                    Result.Message = "Failed to parse data.";
                    break;
                case ClientError.CertError:
                    Result.Message = "Failed server validation.";
                    break;
                case ClientError.AuthError:
                    Result.Message = "Failed server authentication.";
                    break;
            }
            if (string.IsNullOrEmpty(Result.Message))
                Result.Message = "Failed to send request.";

            notification.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Duration = 40000,
                Summary = "Error",
                Detail = Result.Message

            });
        }

        return Result;
    }

}
