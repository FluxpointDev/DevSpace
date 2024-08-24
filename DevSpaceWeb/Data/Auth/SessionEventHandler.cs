using MongoDB.Bson;

namespace DevSpaceWeb.Data.Auth;

public delegate void SessionEventHandler(object sender, ObjectId user, SessionEventType type);

public enum SessionEventType
{
    AccountUpdate, Logout
}
