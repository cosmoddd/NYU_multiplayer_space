using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager.Demos
{

    /// <summary>
    /// This example shows how to load players into the same scene when using scene stacking.
    /// The first two players to enter the green sphere will be placed in the same scene.
    /// The third and following players will be placed in a new, stacked scene.
    /// </summary>
    public class SharedSceneStackingExample : MonoBehaviour
    {
        /// <summary>
        /// Additive scene to load.
        /// </summary>
        [Tooltip("Additive scene to load.")]
        [SerializeField]
        private string _additiveScene;

        /// <summary>
        /// Scenes loaded for connections.
        /// </summary>
        private Dictionary<NetworkConnection, HashSet<Scene>> _loadedScenes = new Dictionary<NetworkConnection, HashSet<Scene>>();


        private void Start()
        {
            FlexSceneManager.OnClientVisibilityChangeEnd += FlexSceneManager_OnClientVisibilityChangeEnd;
        }

        /// <summary>
        /// Received when scene visibilities change for connections.
        /// </summary>
        /// <param name="args"></param>
        private void FlexSceneManager_OnClientVisibilityChangeEnd(ClientVisibilityChangeEventArgs args)
        {
            //Only store which scenes connections are in on the server.
            if (!NetworkServer.active)
                return;
            //No connections to store scenes for.
            if (args.Connections == null)
                return;

            foreach (NetworkConnection nc in args.Connections)
            {
                if (nc == null)
                    continue;

                /* Try to get the hashset for the connection.
                 * The hashset contains which scenes the connection
                 * is in. */
                HashSet<Scene> scenes;
                _loadedScenes.TryGetValue(nc, out scenes);

                //If visibility was added.
                if (args.Added)
                {
                    //No scenes for connection yet, generate hashset and add to loaded scenes.
                    if (scenes == null)
                    {
                        scenes = new HashSet<Scene>();
                        _loadedScenes.Add(nc, scenes);
                    }

                    //Add loaded scenes to hashset.
                    scenes.UnionWith(args.Scenes);
                }
                //If visibility was removed.
                else
                {
                    if (scenes != null)
                    {
                        //Remove loaded scenes.
                        for (int i = 0; i < args.Scenes.Count; i++)
                            scenes.Remove(args.Scenes[i]);

                        //Connection isn't in any more scenes, no need to keep it in the collection.
                        if (scenes.Count == 0)
                            _loadedScenes.Remove(nc);
                    }
                }
            }
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
            if (string.IsNullOrEmpty(_additiveScene))
                return;

            //Additive.
            AdditiveScenesData asd;
            /* If a stacked scene is already loaded then grab the first scene handle. from loaded scenes.
             * You can of course specify any scene handle you wish to load clients into the same scene,
             * while using scene stacking. */
            if (_loadedScenes.Count == 1)
            {
                HashSet<Scene> scenes = _loadedScenes.First().Value;
                Scene firstScene = scenes.First();
                SceneReferenceData srd = new SceneReferenceData()
                {
                    Handle = firstScene.handle,
                    Name = firstScene.name
                };

                asd = new AdditiveScenesData(new SceneReferenceData[] { srd });
            }
            //A stacked scene doesn't exist yet, make a new one.
            else
            {
                /* When loading a stacked scene without using the handle
                 * a new scene will be generated rather than loading into
                 * the existing scene. */
                asd = new AdditiveScenesData(new string[] { _additiveScene });
            }

            /* Stacking requires to be loaded for connection only. */
            FlexSceneManager.LoadConnectionScenes(new NetworkConnection[] { triggeringIdentity.connectionToClient }, null, asd, false, LocalPhysicsMode.Physics3D);
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

            /* If here, not using a return scene. The server will now
             * unload all scenes for this connection. Things will beak
             * on the clients end, but that's part of the demo. */
            List<SceneReferenceData> srds = new List<SceneReferenceData>();
            foreach (Scene item in loadedScenes)
                srds.Add(new SceneReferenceData(item));

            AdditiveScenesData asd = new AdditiveScenesData(srds.ToArray());

            /* Stacking requires to be unloaded for connection only. */
            FlexSceneManager.UnloadConnectionScenes(new NetworkConnection[] { triggeringIdentity.connectionToClient }, asd);
        }

    }


}