using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(NetworkIdentity))]
    public class FlexSceneChecker : NetworkVisibility
    {

        #region Serialized.
        /// <summary>
        /// Enable to force this object to be hidden from all observers.
        /// <para>If this object is a player object, it will not be hidden for that client.</para>
        /// </summary>
        [Tooltip("Enable to force this object to be hidden from all observers. If this object is a player object, it will not be hidden for that client.")]
        [SerializeField]
        private bool _forceHidden = false;
        /// <summary>
        /// True to add this object to whichever scene it is placed or spawned in. Generally best left true for games which will not load the same scene multiple times, eg: loading many game instances on a single server. If you were to leave this true while loading a scene multiple times then the object would be available to all clients since the active seen on the server would be the same, regardless if other scenes were seperated.
        /// </summary>
        [Tooltip("True to add this object to whichever scene it is placed or spawned in. Generally best left true for games which will not load the same scene multiple times, eg: loading many game instances on a single server. If you were to leave this true while loading a scene multiple times then the object would be available to all clients since the active seen on the server would be the same, regardless if other scenes were seperated.")]
        [SerializeField]
        private bool _addToCurrentScene = true;
        #endregion

        #region Private.
        /// <summary>
        /// Scenes this object resides in.
        /// </summary>
        private HashSet<Scene> _currentScenes = new HashSet<Scene>();
        #endregion

        private void OnEnable()
        {
            if (NetworkServer.active)
            {
                FlexSceneManager.AddSceneChecker(this);
            }
        }
        private void OnDisable()
        {
            if (NetworkServer.active)
            {
                FlexSceneManager.RemoveSceneChecker(this);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (_addToCurrentScene)
                _currentScenes.Add(gameObject.scene);
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            if (_addToCurrentScene)
                _currentScenes.Remove(gameObject.scene);
        }

        [Server]
        public void AddedToScene(Scene s)
        {
            if (string.IsNullOrEmpty(s.name))
                return;

            _currentScenes.Add(s);
        }
        public void RemovedFromScene(Scene s)
        {
            if (string.IsNullOrEmpty(s.name))
                return;

            _currentScenes.Remove(s);
        }

        [Server]
        public void ReplacedScene(Scene s)
        {
            if (string.IsNullOrEmpty(s.name))
                return;

            _currentScenes.Clear();
            AddedToScene(s);
        }

        [Server]
        public void RebuildObservers()
        {
            base.netIdentity.RebuildObservers(false);
        }

        /// <summary>
        /// Callback used by the visibility system to determine if an observer (player) can see this object.
        /// <para>If this function returns true, the network connection will be added as an observer.</para>
        /// </summary>
        /// <param name="conn">Network connection of a player.</param>
        /// <returns>True if the player can see this object.</returns>
        public override bool OnCheckObserver(NetworkConnection conn)
        {
            if (_forceHidden)
                return false;

            HashSet<NetworkConnection> sceneConnections;
            /* Get network identities for the scene which this object resides.
             * If the scene is found in the collection return if the network identity
             * for the connection is found in the scene. */
            if (FlexSceneManager.SceneConnections.TryGetValue(gameObject.scene, out sceneConnections))
                return sceneConnections.Contains(conn);

            //Fall through. Scene doesn't exist in collection therefor no identities are added to it.
            return false;
        }

        public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize)
        {
            //Hidden to clients, don't add any observers.
            if (_forceHidden)
                return;


            //For all objects which exist in the same scene as this one, add as an observer.
            HashSet<NetworkConnection> sceneConnections;
            foreach (var item in _currentScenes)
            {
                if (FlexSceneManager.SceneConnections.TryGetValue(item, out sceneConnections))
                {
                    foreach (NetworkConnection conn in sceneConnections)
                    {
                        if (conn != null)
                            observers.Add(conn);
                    }
                }
            }


            ////For all objects which exist in the same scene as this one, add as an observer.
            //HashSet<NetworkConnection> sceneConnections;
            //if (FlexSceneManager.SceneConnections.TryGetValue(gameObject.scene, out sceneConnections))
            //{
            //    foreach (NetworkConnection conn in sceneConnections)
            //    {
            //        if (conn != null)
            //            observers.Add(conn);
            //    }
            //}
        }


    }
}
