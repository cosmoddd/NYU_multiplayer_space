using Mirror;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager.Events
{


    public struct ClientVisibilityChangeEventArgs
    {

        /// <summary>
        /// Scenes on the server which the client visibility has changed.
        /// </summary>
        public readonly List<Scene> Scenes;
        /// <summary>
        /// Connections to clients which the visibility has changed for.
        /// </summary>
        public readonly NetworkConnection[] Connections;
        /// <summary>
        /// True if visibility was added, false if visibility was removed.
        /// </summary>
        public bool Added;

        public ClientVisibilityChangeEventArgs(List<Scene> scenes, NetworkConnection conn, bool added)
        {
            Scenes = scenes;
            Connections = new NetworkConnection[] { conn };
            Added = added;
        }

        public ClientVisibilityChangeEventArgs(List<Scene> scenes, NetworkConnection[] conns, bool added)
        {
            Scenes = scenes;
            Connections = conns;
            Added = added;
        }


    }


}