using recipe_share_api.Controllers;

namespace recipe_share_api;

public interface ISessionState
{
    public SiteSession? Session { get; }

    public void Start(SiteSession session);
    public void Clear();
}

public class SessionState : ISessionState
{
    public SiteSession? Session { get; private set; }

    public void Clear()
    {
        Session = null;
    }

    public void Start(SiteSession session)
    {
        Session = session;
    }
}