
using Mirror;
using System.Collections.Generic;
using System.Linq;

namespace FirstGearGames.FlexSceneManager.LoadUnloadDatas
{


    public class UnloadSceneQueueData
    {
        /// <summary>
        /// Clients which receive this SceneQueueData. If Networked, all clients do. If Connections, only the specified Connections do.
        /// </summary>
        public SceneScopeTypes ScopeType;
        /// <summary>
        /// Additive scenes to unload.
        /// </summary>
        public AdditiveScenesData AdditiveScenes;
        /// <summary>
        /// Current data on networked scenes.
        /// </summary>
        public NetworkedScenesData NetworkedScenes;
        /// <summary>
        /// Connections to unload scenes for. Only valid on the server and when ScopeType is Connections.
        /// </summary>
        [System.NonSerialized]
        public readonly NetworkConnection[] Connections;
        /// <summary>
        /// True if to iterate this queue data as server.
        /// </summary>
        [System.NonSerialized]
        public readonly bool AsServer;


        /// <summary>
        /// Creates an empty SceneQueueData that will serialize over the network.
        /// </summary>
        public UnloadSceneQueueData()
        {
            MakeSerializable();
        }

        /// <summary>
        /// Creates a SceneQueueData.
        /// </summary>
        /// /// <param name="singleScene"></param>
        /// <param name="additiveScenes"></param>
        public UnloadSceneQueueData(SceneScopeTypes scopeType, NetworkConnection[] conns, AdditiveScenesData additiveScenes, NetworkedScenesData networkedScenes, bool asServer)
        {
            ScopeType = scopeType;
            Connections = conns;
            AdditiveScenes = additiveScenes;
            NetworkedScenes = networkedScenes;
            AsServer = asServer;
            MakeSerializable();
        }

        /// <summary>
        /// Ensures all values of this class can be serialized.
        /// </summary>
        public void MakeSerializable()
        {
            //Null additive scenes.
            if (AdditiveScenes == null)
            {
                AdditiveScenes = new AdditiveScenesData()
                {
                    SceneReferenceDatas = new SceneReferenceData[0]
                };
            }
            //Not null additive scenes.
            else
            {
                //Null scene names.
                if (AdditiveScenes.SceneReferenceDatas == null)
                    AdditiveScenes.SceneReferenceDatas = new SceneReferenceData[0];

                List<SceneReferenceData> listSceneReferenceDatas = AdditiveScenes.SceneReferenceDatas.ToList();
                for (int i = 0; i < listSceneReferenceDatas.Count; i++)
                {
                    if (string.IsNullOrEmpty(listSceneReferenceDatas[i].Name))
                    {
                        listSceneReferenceDatas.RemoveAt(i);
                        i--;
                    }
                }
                AdditiveScenes.SceneReferenceDatas = listSceneReferenceDatas.ToArray();
            }

            //Networked scenes.
            if (NetworkedScenes == null)
                NetworkedScenes = new NetworkedScenesData();
        }
    }



}