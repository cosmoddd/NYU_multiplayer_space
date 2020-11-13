using Mirror;

public class LocalPlayerOnlyNetworkBehaviour : NetworkBehaviour
{
    public void OnStartClient()
    {
        if (!isLocalPlayer) Destroy(this);
    }
}