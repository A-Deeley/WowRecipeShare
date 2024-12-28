using Microsoft.Extensions.Primitives;
using recipe_share_api.Controllers;
using recipe_share_api.Login;

namespace recipe_share_api.Sessions;

public interface ISessionState
{
    public const string SessionIdHeader = "X-RecipeShare-SessionId";
    public Guid Start(SiteSession session);
    public bool TryGetSession(Guid sessionId, out SiteSession? session);
    public bool TryGetSession(HttpRequest request, out SiteSession? session);
    public void Clear(Guid sessionId);
}

public class SessionState : ISessionState
{
    Dictionary<Guid, SiteSession> _sessions = new();

    public void Clear(Guid sessionId)
    {
        _sessions.Remove(sessionId);
    }

    public Guid Start(SiteSession session)
    {
        Guid sessionId = Guid.NewGuid();
        _sessions.Add(sessionId, session);

        return sessionId;
    }

    public bool TryGetSession(Guid sessionId, out SiteSession? session)
    {
        bool exists = _sessions.TryGetValue(sessionId, out session);

        if (!exists) return false;

        bool sessionNullOrExpired = session is null || session.ExpiresOn <= DateTime.Now;
        if (sessionNullOrExpired)
        {
            _sessions.Remove(sessionId);
            return false;
        }

        return true;
    }

    public bool TryGetSession(HttpRequest request, out SiteSession? session)
    {
        session = null;

        request.Headers.TryGetValue(ISessionState.SessionIdHeader, out StringValues value);
        var sessionIdHeaderValue = value.FirstOrDefault();
        if (!Guid.TryParse(sessionIdHeaderValue, out var sessionIdValue))
            return false;

        return TryGetSession(sessionIdValue, out session);
    }
}