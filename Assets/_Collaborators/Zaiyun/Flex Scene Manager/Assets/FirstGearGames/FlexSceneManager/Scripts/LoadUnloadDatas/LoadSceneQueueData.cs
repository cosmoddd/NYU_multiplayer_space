using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager.LoadUnloadDatas
{


    /// <summary>
    /// Used to load a scene for a targeted connection.
    /// </summary>
    public class LoadSceneQueueData
    {
        /// <summary>
        /// Clients which receive this SceneQueueData. If Networked, all clients do. If Connections, only the specified Connections do.
        /// </summary>
        public SceneScopeTypes ScopeType;
        /// <summary>
        /// True if to only load scenes which are not yet loaded. When false a scene may load multiple times.
        /// </summary>
        public bool LoadOnlyUnloaded;
        /// <summary>
        /// Single scene to load.
        /// </summary>
        public SingleSceneData SingleScene;
        /// <summary>
        /// Additive scenes to load.
        /// </summary>
        public AdditiveScenesData AdditiveScenes;
        /// <summary>
        /// Current data on networked scenes.
        /// </summary>
        public NetworkedScenesData NetworkedScenes;
        /// <summary>
        /// Connections to load scenes for. Only valid on the server and when ScopeType is Connections.
        /// </summary>
        [System.NonSerialized]
        public readonly NetworkConnection[] Connections;
        /// <summary>
        /// Parameters which can be passed into a scene load. These are included with all scene events when dispatching on server. Params can be useful to link personalized data with scene load callbacks, such as a match Id.
        /// </summary>
        [System.NonSerialized]
        public readonly object[] Params = null;
        /// <summary>
        /// True if to iterate this queue data as server.
        /// </summary>
        [System.NonSerialized]
        public readonly bool AsServer;
        /// <summary>
        /// Physics mode to use when loading this scene. Only used by the server.
        /// </summary>
        [System.NonSerialized]
        public readonly LocalPhysicsMode LocalPhysics = LocalPhysicsMode.None;

        /// <summary>
        /// Creates an empty SceneQueueData that will serialize over the network.
        /// </summary>
        public LoadSceneQueueData()
        {
            MakeSerializable();
        }

        /// <summary>
        /// Creates a SceneQueueData.
        /// </summary>
        /// /// <param name="singleScene"></param>
        /// <param name="additiveScenes"></param>
        /// <param name="loadOnlyUnloaded"></param>
        public LoadSceneQueueData(SceneScopeTypes scopeType, NetworkConnection[] conns, SingleSceneData singleScene, AdditiveScenesData additiveScenes, bool loadOnlyUnloaded, LocalPhysicsMode localPhysics, NetworkedScenesData networkedScenes, bool asServer)
        {
            ScopeType = scopeType;
            SingleScene = singleScene;
            AdditiveScenes = additiveScenes;
            LoadOnlyUnloaded = loadOnlyUnloaded;
            NetworkedScenes = networkedScenes;
            Connections = conns;
            AsServer = asServer;
            LocalPhysics = localPhysics;

            MakeSerializable();
        }

        /// <summary>
        /// Ensures all values of this class can be serialized.
        /// </summary>
        public void MakeSerializable()
        {
            //Null single scene.
            if (SingleScene == null)
            {
                SingleScene = new SingleSceneData()
                {
                    SceneReferenceData = new SceneReferenceData(),
                    MovedNetworkIdentities = new NetworkIdentity[0]
                };
            }
            //Not null single scene.
            else
            {
                //Moved identities is null.
                if (SingleScene.MovedNetworkIdentities == null)
                {
                    SingleScene.MovedNetworkIdentities = new NetworkIdentity[0];
                }
                //Has moved identities.
                else
                {
                    //Remove null of unset network identities.
                    List<NetworkIdentity> listMovedIdentities = SingleScene.MovedNetworkIdentities.ToList();
                    for (int i = 0; i < listMovedIdentities.Count; i++)
                    {
                        if (listMovedIdentities[i] == null || listMovedIdentities[i].netId == 0)
                        {
                            listMovedIdentities.RemoveAt(i);
                            i--;
                        }
                    }
                    SingleScene.MovedNetworkIdentities = listMovedIdentities.ToArray();
                }

            }

            //Null additive scenes.
            if (AdditiveScenes == null)
            {
                AdditiveScenes = new AdditiveScenesData();
            }
            //Not null additive scenes.
            else
            {
                //Null scene datas.
                if (AdditiveScenes.SceneReferenceDatas == null)
                    AdditiveScenes.SceneReferenceDatas = new SceneReferenceData[0];
            }

            //Networked scenes.
            if (NetworkedScenes == null)
                NetworkedScenes = new NetworkedScenesData();
        }

    }


}