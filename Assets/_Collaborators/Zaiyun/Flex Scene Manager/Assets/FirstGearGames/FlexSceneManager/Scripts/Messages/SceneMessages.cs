using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using Mirror;

namespace FirstGearGames.FlexSceneManager.Messages
{

    /// <summary>
    /// Sent to clients to load networked scenes.
    /// </summary>
    public class LoadScenesMessage : NetworkMessage
    {
        public LoadSceneQueueData SceneQueueData;
    }


    /// <summary>
    /// Sent to clients to unload networked scenes.
    /// </summary>
    public class UnloadScenesMessage : NetworkMessage
    {
        public UnloadSceneQueueData SceneQueueData;
    }


    /// <summary>
    /// Sent to server to indicate which scenes a client has loaded.
    /// </summary>
    public class ClientScenesLoadedMessage : NetworkMessage
    {
        public SceneReferenceData[] SceneDatas;
    }

    public class ClientPlayerCreated : NetworkMessage
    {

    }

}