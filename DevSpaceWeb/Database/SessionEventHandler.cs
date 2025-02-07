using DevSpaceWeb.Data.Users;
using MongoDB.Bson;

namespace DevSpaceWeb.Database;

public delegate void SessionEventHandler(object sender, ObjectId user, SessionEventType type);

public delegate void NotificationEventHandler(Notification notification);

public enum SessionEventType
{
    AccountUpdate, Logout
}
