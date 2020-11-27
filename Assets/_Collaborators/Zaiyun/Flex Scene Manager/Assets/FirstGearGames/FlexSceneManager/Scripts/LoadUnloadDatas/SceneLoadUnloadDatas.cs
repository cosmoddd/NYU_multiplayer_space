using Mirror;

namespace FirstGearGames.FlexSceneManager.LoadUnloadDatas
{


    public class SingleSceneData
    {
        /// <summary>
        /// SceneReferenceData for each scene to load.
        /// </summary>
        public SceneReferenceData SceneReferenceData;
        /// <summary>
        /// NetworkIdentities to move to the new single scene.
        /// </summary>
        public NetworkIdentity[] MovedNetworkIdentities;

        public SingleSceneData()
        {
            MovedNetworkIdentities = new NetworkIdentity[0];
        }

        public SingleSceneData(string sceneName)
        {
            SceneReferenceData = new SceneReferenceData() { Name = sceneName };
            MovedNetworkIdentities = new NetworkIdentity[0];
        }
        public SingleSceneData(SceneReferenceData sceneReferenceData)
        {
            SceneReferenceData = sceneReferenceData;
            MovedNetworkIdentities = new NetworkIdentity[0];
        }
        public SingleSceneData(string sceneName, NetworkIdentity[] movedNetworkIdentities)
        {
            SceneReferenceData = new SceneReferenceData() { Name = sceneName };
            MovedNetworkIdentities = movedNetworkIdentities;
        }
        public SingleSceneData(SceneReferenceData sceneReferenceData, NetworkIdentity[] movedNetworkIdentities)
        {
            SceneReferenceData = sceneReferenceData;
            MovedNetworkIdentities = movedNetworkIdentities;
        }
    }

    public class AdditiveScenesData
    {
        /// <summary>
        /// SceneReferenceData for each scene to load.
        /// </summary>
        public SceneReferenceData[] SceneReferenceDatas;

        public AdditiveScenesData()
        {
            SceneReferenceDatas = new SceneReferenceData[0];
        }

        public AdditiveScenesData(string[] sceneNames)
        {
            SceneReferenceDatas = new SceneReferenceData[sceneNames.Length];
            for (int i = 0; i < sceneNames.Length; i++)
                SceneReferenceDatas[i] = new SceneReferenceData { Name = sceneNames[i] };
        }
        public AdditiveScenesData(SceneReferenceData[] sceneReferenceDatas)
        {
            SceneReferenceDatas = sceneReferenceDatas;
        }
    }


}