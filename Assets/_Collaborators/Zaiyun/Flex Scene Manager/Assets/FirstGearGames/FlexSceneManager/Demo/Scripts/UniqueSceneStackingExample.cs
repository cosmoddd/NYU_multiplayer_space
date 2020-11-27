using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager.Demos
{

    /// <summary>
    /// This will load every client which enters into their own stacked scene.
    /// </summary>
    public class UniqueSceneStackingExample : MonoBehaviour
    {
        /// <summary>
        /// Single scene to load. Leave empty to not load a single scene.
        /// </summary>
        [Tooltip("Single scene to load. Leave empty to not load a single scene.")]
        [SerializeField]
        private string _singleScene;
        /// <summary>
        /// True to move the triggering identity.
        /// </summary>
        [Tooltip("True to move the triggering identity.")]
        [SerializeField]
        private bool _moveIdentity = true;
        /// <summary>
        /// Additive scenes to load. Leave empty to not load additive scenes.
        /// </summary>
        [Tooltip("Additive scenes to load. Leave empty to not load additive scenes.")]
        [SerializeField]
        private string[] _additiveScenes;

        /// <summary>
        /// Scenes loaded for connections.
        /// </summary>
        private Dictionary<NetworkConnection, HashSet<Scene>> _loadedScenes = new Dictionary<NetworkConnection, HashSet<Scene>>();


        private void Start()
        {
            FlexSceneManager.OnLoadSceneEnd += FlexSceneManager_OnLoadSceneEnd;
            FlexSceneManager.OnUnloadSceneEnd += FlexSceneManager_OnUnloadSceneEnd;
        }

        private void FixedUpdate()
        {
            List<NetworkConnection> connectionsWithNullOrNoScenes = new List<NetworkConnection>();
            /* Simulate physics on each loaded scene. When using local physics scenes you
             * must do this otherwise physics will not tick. Be mindful of PhysicsScene casts as well:
             * https://docs.unity3d.com/2019.1/Documentation/ScriptReference/PhysicsScene.html */
            foreach (KeyValuePair<NetworkConnection, HashSet<Scene>> item in _loadedScenes)
            {
                //No scenes for connection.
                if (item.Value.Count == 0)
                {
                    connectionsWithNullOrNoScenes.Add(item.Key);
                    continue;
                }

                foreach (Scene s in item.Value)
                {
                    //If scene exist then simulate physics.
                    if (!string.IsNullOrEmpty(s.name))
                        s.GetPhysicsScene().Simulate(Time.deltaTime);
                    //Scene doesn't exist, queue it to be cleaned from connection.
                    else
                        connectionsWithNullOrNoScenes.Add(item.Key);
                }
            }

            for (int i = 0; i < connectionsWithNullOrNoScenes.Count; i++)
                CleanEmptyScenesFromLoaded(connectionsWithNullOrNoScenes[i]);
        }

        /// <summary>
        /// Received when scene loading ends.
        /// </summary>
        /// <param name="obj"></param>
        private void FlexSceneManager_OnLoadSceneEnd(LoadSceneEndEventArgs args)
        {
            //Only server will manage loaded scenes.
            if (!NetworkServer.active)
                return;
            //Not scoped for connections, or no connections specified.
            if (args.RawData.ScopeType != SceneScopeTypes.Connections || args.RawData.Connections == null)
                return;

            foreach (NetworkConnection nc in args.RawData.Connections)
            {
                if (nc == null)
                    continue;

                HashSet<Scene> scenes;
                //If entry doesn't exist yet.
                if (!_loadedScenes.TryGetValue(nc, out scenes))
                {
                    scenes = new HashSet<Scene>();
                    _loadedScenes.Add(nc, scenes);
                }
                //Add loaded scenes.
                for (int i = 0; i < args.LoadedScenes.Length; i++)
                    scenes.Add(args.LoadedScenes[i]);
            }
        }

        /// <summary>
        /// Received when a scene unloading ends.
        /// </summary>
        /// <param name="args"></param>
        private void FlexSceneManager_OnUnloadSceneEnd(UnloadSceneEndEventArgs args)
        {
            //Only server will manage loaded scenes.
            if (!NetworkServer.active)
                return;
            //No need to process for networked scenes since this is for stacking scenes example, which will always be connection scenes.
            if (args.RawData.ScopeType != SceneScopeTypes.Connections)
                return;

            if (args.RawData.Connections != null)
            {
                for (int i = 0; i < args.RawData.Connections.Length; i++)
                    CleanEmptyScenesFromLoaded(args.RawData.Connections[i]);
            }
        }


        /// <summary>
        /// Removes empty scenes from loaded scenes.
        /// </summary>
        private void CleanEmptyScenesFromLoaded(NetworkConnection conn)
        {
            if (conn == null)
            {
                _loadedScenes.Remove(conn);
                return;
            }

            HashSet<Scene> scenes;
            if (_loadedScenes.TryGetValue(conn, out scenes))
            {
                List<Scene> removeEntries = new List<Scene>();
                foreach (Scene s in scenes)
                {
                    if (string.IsNullOrEmpty(s.name))
                        removeEntries.Add(s);
                }

                for (int i = 0; i < removeEntries.Count; i++)
                    scenes.Remove(removeEntries[i]);
            }

            //If no more scenes remove connection reference.
            if (scenes != null && scenes.Count == 0)
                _loadedScenes.Remove(conn);
        }


        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            LoadScene(other.GetComponent<NetworkIdentity>());
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            UnloadScene(other.GetComponent<NetworkIdentity>());
        }

        private void LoadScene(NetworkIdentity triggeringIdentity)
        {
            if (triggeringIdentity == null)
                return;

            SingleSceneData ssd = null;
            //If to load a single scene.
            if (_singleScene != string.Empty)
            {
                List<NetworkIdentity> movedIdents = new List<NetworkIdentity>();
                if (_moveIdentity)
                    movedIdents.Add(triggeringIdentity);

                ssd = new SingleSceneData(_singleScene, movedIdents.ToArray());
            }

            //Additive.
            AdditiveScenesData asd = null;
            if (_additiveScenes != null && _additiveScenes.Length > 0)
                asd = new AdditiveScenesData(_additiveScenes);

            //Load for connection only.
            FlexSceneManager.LoadConnectionScenes(new NetworkConnection[] { triggeringIdentity.connectionToClient }, ssd, asd, false, LocalPhysicsMode.Physics3D);
        }

        private void UnloadScene(NetworkIdentity triggeringIdentity)
        {
            if (triggeringIdentity == null)
                return;
            /* Dictionary doesn't have key for this connection which means it has
             * no knowledge of which scenes were loaded for it. Because of this,
             * scenes cannot be unloaded. */
            HashSet<Scene> loadedScenes;
            if (!_loadedScenes.TryGetValue(triggeringIdentity.connectionToClient, out loadedScenes))
                return;

            /* The server will now
             * unload all scenes for this connection. */
            List<SceneReferenceData> srds = new List<SceneReferenceData>();
            foreach (Scene item in loadedScenes)
                srds.Add(new SceneReferenceData(item));

            AdditiveScenesData asd = new AdditiveScenesData(srds.ToArray());
            FlexSceneManager.UnloadConnectionScenes(new NetworkConnection[] { triggeringIdentity.connectionToClient }, asd);
        }

    }


}