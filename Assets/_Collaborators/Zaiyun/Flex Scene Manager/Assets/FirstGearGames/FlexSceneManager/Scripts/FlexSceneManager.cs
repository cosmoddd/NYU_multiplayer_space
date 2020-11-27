using FirstGearGames.FlexSceneManager.Events;
using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using FirstGearGames.FlexSceneManager.Messages;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager
{

    public class FlexSceneManager : MonoBehaviour
    {
        #region Types.
        /// <summary>
        /// Data about a scene which is to be loaded. Generated when processing scene queue data.
        /// </summary>
        private class LoadableScene
        {
            public LoadableScene(string sceneName, LoadSceneMode loadMode)
            {
                SceneName = sceneName;
                LoadMode = loadMode;
            }

            public readonly string SceneName;
            public readonly LoadSceneMode LoadMode;
        }
        #endregion

        #region Public.
        /// <summary>
        /// Dispatched when a scene change queue has begun. This will only call if a scene has succesfully begun to load or unload. The queue may process any number of scene events. For example: if a scene is told to unload while a load is still in progress, then the unload will be placed in the queue.
        /// </summary>
        public static event Action OnSceneQueueStart;
        /// <summary>
        /// Dispatched when the scene queue is emptied.
        /// </summary>
        public static event Action OnSceneQueueEnd;
        /// <summary>
        /// Dispatched when a scene load starts.
        /// </summary>
        public static event Action<LoadSceneStartEventArgs> OnLoadSceneStart;
        /// <summary>
        /// Dispatched when completion percentage changes while loading a scene. Value is between 0f and 1f, while 1f is 100% done. Can be used for custom progress bars when loading scenes.
        /// </summary>
        public static event Action<LoadScenePercentEventArgs> OnLoadScenePercentChange;
        /// <summary>
        /// Dispatched when a scene load ends.
        /// </summary>
        public static event Action<LoadSceneEndEventArgs> OnLoadSceneEnd;
        /// <summary>
        /// Dispatched when a scene load starts.
        /// </summary>
        public static event Action<UnloadSceneStartEventArgs> OnUnloadSceneStart;
        /// <summary>
        /// Dispatched when a scene load ends.
        /// </summary>
        public static event Action<UnloadSceneEndEventArgs> OnUnloadSceneEnd;
        /// <summary>
        /// Dispatched when server is about to change scene visibility for a client.
        /// </summary>
        public static event Action<ClientVisibilityChangeEventArgs> OnClientVisibilityChangeStart;
        /// <summary>
        /// Dispatched after the server has changed scene visibility for a client.
        /// </summary>
        public static event Action<ClientVisibilityChangeEventArgs> OnClientVisibilityChangeEnd;

        #endregion

        #region Private.
        /// <summary>
        /// Singleton reference of this script.
        /// </summary>
        private static FlexSceneManager _instance;
        /// <summary>
        /// Scenes which are currently loaded as networked scenes. All players should have networked scenes loaded.
        /// </summary>
        private NetworkedScenesData _networkedScenes = new NetworkedScenesData();
        /// <summary>
        /// Scenes to load or unload, in order.
        /// </summary>
        private List<object> _queuedSceneOperations = new List<object>();
        /// <summary>
        /// Collection of FlexSceneCheckers that can change scenes.
        /// </summary>
        private HashSet<FlexSceneChecker> _sceneCheckers = new HashSet<FlexSceneChecker>();
        /// <summary>
        /// 
        /// </summary>
        private Dictionary<Scene, HashSet<NetworkConnection>> _sceneConnections = new Dictionary<Scene, HashSet<NetworkConnection>>();
        /// <summary>
        /// Scenes which connections are registered as existing.
        /// </summary>
        public static Dictionary<Scene, HashSet<NetworkConnection>> SceneConnections { get { return _instance._sceneConnections; } }
        /// <summary>
        /// Scene containing moved objects when changing single scene. On client this will contain all objects moved until the server destroys them.
        /// Mirror only sends spawn messages once per-client, per server side scene load. If a scene load is performed only for specific connections
        /// then the server is not resetting their single scene, but rather the single scene for those connections only. Because of this, any objects
        /// which are to be moved will not receive a second respawn message, as they are never destroyed on server, only on client.
        /// While on server only this scene contains objects being moved temporarily, before being moved to the new scene.
        /// </summary>
        private Scene _movedObjectsScene;
        /// <summary>
        /// Becomes true when client receives the initial scene load message.
        /// </summary>
        private bool _receivedInitialLoad = false;
        /// <summary>
        /// Default value for auto create player.
        /// When FlexSceneManager starts it will set autoCreatePlayer to false,
        /// while storing the default value. When a client completes their first scene
        /// load the player will be spawned in if autoCreatePlayer was previously true.
        /// </summary>
        private bool _defaultAutoCreatePlayer;
        /// <summary>
        /// Becomes true when when a scene first successfully begins to load or unload. Value is reset to false when the scene queue is emptied.
        /// </summary>
        private bool _sceneQueueStartInvoked = false;
        #endregion

        #region Unity callbacks and initialization.
        /// <summary>
        /// Initializes this script for use. Should only be completed once.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void FirstInitialize()
        {
            GameObject go = new GameObject();
            go.name = "FlexSceneManager";
            go.AddComponent<FlexSceneManager>();
            DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning("Duplicate FlexSceneManager found. You do not need to add FlexSceneManager to your scene; it will be loaded automatically.");
                Destroy(this);
                return;
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
        }

        private void OnEnable()
        {
            NetworkClient.ReplaceHandler<LoadScenesMessage>(OnLoadScenes);
            NetworkClient.ReplaceHandler<UnloadScenesMessage>(OnUnloadScenes);
            NetworkServer.ReplaceHandler<ClientScenesLoadedMessage>(OnClientScenesLoaded);
            NetworkServer.ReplaceHandler<ClientPlayerCreated>(OnClientPlayerCreated);
        }

        private void OnDisable()
        {
            NetworkClient.UnregisterHandler<LoadScenesMessage>();
            NetworkClient.UnregisterHandler<UnloadScenesMessage>();
            NetworkServer.UnregisterHandler<ClientScenesLoadedMessage>();
        }

        private void Start()
        {
            _defaultAutoCreatePlayer = NetworkManager.singleton.autoCreatePlayer;
            NetworkManager.singleton.autoCreatePlayer = false;
        }
        #endregion

        #region Synchronizing late joiners.
        /// <summary>
        /// Called when a client connects to the server, after authentication.
        /// </summary>
        /// <param name="conn"></param>
        public static void OnServerConnect(NetworkConnection conn)
        {
            _instance.OnServerConnectInternal(conn);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        private void OnServerConnectInternal(NetworkConnection conn)
        {
            /* If connection is null and not client host then
             * message cannot be sent. 
             * If client host message still wont be sent but
             * it will be simulated locally. */
            bool connectionEmpty = (conn == null || conn.connectionId == 0);
            if (connectionEmpty && (!NetworkServer.active && !NetworkClient.active))
                return;

            /* If there are no networked scenes then we must still send
             * an empty initial scene load so the client knows they're
             * caught up on scenes when first connecting. */
            if (_networkedScenes.Single.Length == 0 && _networkedScenes.Additive.Length == 0)
            {
                LoadSceneQueueData emptySqd = new LoadSceneQueueData();
                //Send message to load the networked scenes.
                LoadScenesMessage emptyMsg = new LoadScenesMessage()
                {
                    SceneQueueData = emptySqd
                };

                if (!connectionEmpty)
                    conn.Send(emptyMsg);
                else
                    OnLoadScenes(null, emptyMsg);

                return;
            }

            SingleSceneData ssd = null;
            //If a single scene exist.
            if (_networkedScenes.Single.Length > 0)
                ssd = new SingleSceneData(_networkedScenes.Single);

            AdditiveScenesData asd = null;
            //If additive scenes exist.
            if (_networkedScenes.Additive.Length > 0)
                asd = new AdditiveScenesData(_networkedScenes.Additive);

            /* Client will only load what is unloaded. This is so
             * if they are on the scene with the networkmanager or other
             * ddols, the ddols wont be loaded multiple times. */
            LoadSceneQueueData sqd = new LoadSceneQueueData(SceneScopeTypes.Networked, null, ssd, asd, true, LocalPhysicsMode.None, _networkedScenes, false);

            //Send message to load the networked scenes.
            LoadScenesMessage msg = new LoadScenesMessage()
            {
                SceneQueueData = sqd
            };

            if (!connectionEmpty)
                conn.Send(msg);
            else
                OnLoadScenes(null, msg);
        }
        #endregion

        #region Player disconnect.
        /// <summary>
        /// Received when a player disconnects from the server.
        /// </summary>
        /// <param name="conn"></param>
        [Server]
        public static void OnServerDisconnect(NetworkConnection conn)
        {
            _instance.OnServerDisconnectInternal(conn);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        private void OnServerDisconnectInternal(NetworkConnection conn)
        {
            //Scenes to unload because there are no more observers.
            List<SceneReferenceData> unloadSceneReferenceDatas = new List<SceneReferenceData>();
            //Current active scene.
            Scene activeScene = SceneManager.GetActiveScene();
            //Remove connection from all connection scenes.
            foreach (KeyValuePair<Scene, HashSet<NetworkConnection>> item in SceneConnections)
            {
                item.Value.Remove(conn);
                //No more connections and not a networked scene.
                if (item.Value.Count == 0 && !IsNetworkedScene(item.Key.name, _networkedScenes))
                {
                    //If not the active seen then add to be unloaded.
                    if (item.Key != activeScene)
                    {
                        SceneReferenceData sd = new SceneReferenceData()
                        {
                            Handle = item.Key.handle,
                            Name = item.Key.name
                        };
                        unloadSceneReferenceDatas.Add(sd);
                    }
                }
            }

            //If at least one scene should be unloaded.
            if (unloadSceneReferenceDatas.Count > 0)
            {
                AdditiveScenesData asd = new AdditiveScenesData()
                {
                    SceneReferenceDatas = unloadSceneReferenceDatas.ToArray()
                };

                UnloadConnectionScenes(null, asd);
            }
        }
        #endregion

        #region Player creation.
        /// <summary>
        /// Called on the client to tell the server when the client's player has been created. Typically called after performing ClientScene.AddPlayer().
        /// </summary>
        public static void SendPlayerCreated()
        {
            _instance.SendPlayerCreatedInternal();
        }
        /// <summary>
        /// Called on the client to tell the server when the client's player has been created. Typically called after performing ClientScene.AddPlayer().
        /// </summary>
        private void SendPlayerCreatedInternal()
        {
            NetworkClient.Send(new ClientPlayerCreated());
        }
        /// <summary>
        /// Resets that initial load has been completed. Must be called when connecting to reset that initial scenes have been loaded, as well to register messages again.
        /// </summary>
        [Client]
        public static void ResetInitialLoad()
        {
            _instance._receivedInitialLoad = false;
        }
        #endregion

        #region Server received messages.
        /// <summary>
        /// Received on the server immediately after the client request their player to be spawned.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnClientPlayerCreated(NetworkConnection conn, ClientPlayerCreated msg)
        {
            if (conn == null || conn.identity == null)
            {
                Debug.LogWarning("Connection or identity on connection is null.");
                return;
            }

            //Add to single scene.
            Scene s;
            if (!string.IsNullOrEmpty(_networkedScenes.Single))
            {
                s = SceneManager.GetSceneByName(_networkedScenes.Single);
                if (!string.IsNullOrEmpty(s.name))
                    AddToScene(s, conn);
            }
            if (_networkedScenes.Additive != null)
            {
                for (int i = 0; i < _networkedScenes.Additive.Length; i++)
                {
                    s = SceneManager.GetSceneByName(_networkedScenes.Additive[i]);
                    if (!string.IsNullOrEmpty(s.name))
                        AddToScene(s, conn);
                }
            }
        }
        /// <summary>
        /// Received on server when a client loads scenes.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnClientScenesLoaded(NetworkConnection conn, ClientScenesLoadedMessage msg)
        {
            List<Scene> scenesLoaded = new List<Scene>();
            //Build scenes for events.
            foreach (SceneReferenceData item in msg.SceneDatas)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    Scene s;
                    //If handle exist then get scene by the handle.
                    if (item.Handle != 0)
                        s = GetSceneByHandle(item.Handle);
                    //Otherwise get it by the name.
                    else
                        s = SceneManager.GetSceneByName(item.Name);

                    if (!string.IsNullOrEmpty(s.name))
                        scenesLoaded.Add(s);
                }
            }

            /* Visibility start event. */
            ClientVisibilityChangeEventArgs args = new ClientVisibilityChangeEventArgs(scenesLoaded, conn, true);
            OnClientVisibilityChangeStart?.Invoke(args);

            //Add to scenes.
            for (int i = 0; i < scenesLoaded.Count; i++)
                AddToScene(scenesLoaded[i], conn);

            /* Visibility end event. */
            OnClientVisibilityChangeEnd?.Invoke(args);
        }
        #endregion

        #region Events.
        /// <summary>
        /// Checks if OnQueueStart should invoke, and if so invokes.
        /// </summary>
        private void TryInvokeOnQueueStart()
        {
            if (_sceneQueueStartInvoked)
                return;

            _sceneQueueStartInvoked = true;
            OnSceneQueueStart?.Invoke();
        }
        /// <summary>
        /// Checks if OnQueueEnd should invoke, and if so invokes.
        /// </summary>
        private void TryInvokeOnQueueEnd()
        {
            if (!_sceneQueueStartInvoked)
                return;

            _sceneQueueStartInvoked = false;
            OnSceneQueueEnd?.Invoke();
        }
        /// <summary>
        /// Invokes that a scene load has started. Only called when valid scenes will be loaded.
        /// </summary>
        /// <param name="sqd"></param>
        private void InvokeOnSceneLoadStart(LoadSceneQueueData sqd)
        {
            TryInvokeOnQueueStart();
            OnLoadSceneStart?.Invoke(new LoadSceneStartEventArgs(sqd));
        }
        /// <summary>
        /// Invokes that a scene load has ended. Only called after a valid scene has loaded.
        /// </summary>
        /// <param name="sqd"></param>
        private void InvokeOnSceneLoadEnd(LoadSceneQueueData sqd, List<string> requestedLoadScenes, List<Scene> loadedScenes)
        {
            //Make new list to not destroy original data.
            List<string> skippedScenes = requestedLoadScenes.ToList();
            //Remove loaded scenes from requested scenes.
            for (int i = 0; i < loadedScenes.Count; i++)
                skippedScenes.Remove(loadedScenes[i].name);

            LoadSceneEndEventArgs args = new LoadSceneEndEventArgs(sqd, loadedScenes.ToArray(), skippedScenes.ToArray());
            OnLoadSceneEnd?.Invoke(args);
        }
        /// <summary>
        /// Invokes that a scene unload has started. Only called when valid scenes will be unloaded.
        /// </summary>
        /// <param name="sqd"></param>
        private void InvokeOnSceneUnloadStart(UnloadSceneQueueData sqd)
        {
            TryInvokeOnQueueStart();
            OnUnloadSceneStart?.Invoke(new UnloadSceneStartEventArgs(sqd));
        }
        /// <summary>
        /// Invokes that a scene unload has ended. Only called after a valid scene has unloaded.
        /// </summary>
        /// <param name="sqd"></param>
        private void InvokeOnSceneUnloadEnd(UnloadSceneQueueData sqd)
        {
            OnUnloadSceneEnd?.Invoke(new UnloadSceneEndEventArgs(sqd));
        }
        /// <summary>
        /// Invokes when completion percentage changes while unloading or unloading a scene. Value is between 0f and 1f, while 1f is 100% done.
        /// </summary>
        /// <param name="value"></param>
        private void InvokeOnScenePercentChange(LoadSceneQueueData sqd, float value)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            OnLoadScenePercentChange?.Invoke(new LoadScenePercentEventArgs(sqd, value));
        }
        #endregion

        #region Scene queue processing.
        /// <summary>
        /// Processes queued scene operations.
        /// </summary>
        /// <param name="asServer"></param>
        /// <returns></returns>
        private IEnumerator __ProcessSceneQueue()
        {
            /* Queue start won't invoke unless a scene load or unload actually occurs.
             * For example: if a scene is already loaded, and nothing needs to be loaded,
             * queue start will not invoke. */

            while (_queuedSceneOperations.Count > 0)
            {
                //If a load scene.
                if (_queuedSceneOperations[0] is LoadSceneQueueData)
                    yield return StartCoroutine(__LoadScenes());
                //If an unload scene.
                else if (_queuedSceneOperations[0] is UnloadSceneQueueData)
                    yield return StartCoroutine(__UnloadScenes());

                _queuedSceneOperations.RemoveAt(0);
            }

            /* AutoCreatePlayer.
             * If this is the first time a scene load is being called on client
             * then the client can auto create their player after all scene
             * loads have been processed. */
            if (NetworkClient.active && !_receivedInitialLoad)
            {
                _receivedInitialLoad = true;

                //If auto create was defaulted to true then create player.
                if (_defaultAutoCreatePlayer)
                {
                    ClientScene.AddPlayer(NetworkClient.connection);
                    SendPlayerCreatedInternal();
                }
            }

            TryInvokeOnQueueEnd();
        }
        #endregion

        #region LoadScenes
        /// <summary>
        /// Loads scenes which all clients will be synchronized into.
        /// </summary>
        /// <param name="singleScene">Single scene to load. Use null to opt-out of single scene loading.</param>
        /// <param name="additiveScenes">Additive scenes to load. Use null to opt-out of additive scene loading.</param>
        [Server]
        public static void LoadNetworkedScenes(SingleSceneData singleScene, AdditiveScenesData additiveScenes)
        {
            _instance.LoadScenesInternal(SceneScopeTypes.Networked, null, singleScene, additiveScenes, true, LocalPhysicsMode.None, _instance._networkedScenes, true);
        }
        /// <summary>
        /// Loads scenes on server and tells connections to load them as well. Other connections will not load this scene.
        /// </summary>
        /// <param name="conns">Connections to load scenes for.</param>
        /// <param name="singleScene">Single scene to load. Use null to opt-out of single scene loading.</param>
        /// <param name="additiveScenes">Additive scenes to load. Use null to opt-out of additive scene loading.</param>
        /// <param name="loadOnlyUnloaded">True to only load scenes which are currently not loaded. Setting this to false will allow the server to load the same scene multiple times, but it will only load once on clients.</param>
        /// <param name="localPhysics">LocalPhysicsMode to use for the loaded scenes.</param>
        [Server]
        public static void LoadConnectionScenes(NetworkConnection[] conns, SingleSceneData singleScene, AdditiveScenesData additiveScenes, bool loadOnlyUnloaded = true, LocalPhysicsMode localPhysics = LocalPhysicsMode.None)
        {
            _instance.LoadScenesInternal(SceneScopeTypes.Connections, conns, singleScene, additiveScenes, loadOnlyUnloaded, localPhysics, _instance._networkedScenes, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="singleScene"></param>
        /// <param name="additiveScenes"></param>
        /// <param name="loadOnlyUnloaded"></param>
        /// <param name="asServer"></param>
        private void LoadScenesInternal(SceneScopeTypes scope, NetworkConnection[] conns, SingleSceneData singleScene, AdditiveScenesData additiveScenes, bool loadOnlyUnloaded, LocalPhysicsMode localPhysics, NetworkedScenesData networkedScenes, bool asServer)
        {
            //Add to scene queue data.        
            _queuedSceneOperations.Add(new LoadSceneQueueData(scope, conns, singleScene, additiveScenes, loadOnlyUnloaded, localPhysics, networkedScenes, asServer));
            /* If only one entry then scene operations are not currently in progress.
             * Should there be more than one entry then scene operations are already 
             * occuring. The coroutine will automatically load in order. */

            if (_queuedSceneOperations.Count == 1)
                StartCoroutine(__ProcessSceneQueue());
        }

        /// <summary>
        /// Loads a connection scene queue data. This behaves just like a networked scene load except it sends only to the specified connections, and it always loads as an additive scene on server.
        /// </summary>
        /// <returns></returns>
        private IEnumerator __LoadScenes()
        {
            LoadSceneQueueData sqd = _queuedSceneOperations[0] as LoadSceneQueueData;
            RemoveInvalidSceneQueueData(ref sqd);
            /* No single or additive scene data. They were
             * empty or removed due to being invalid. */
            if (sqd.SingleScene == null && sqd.AdditiveScenes == null)
                yield break;
            /* It's safe to assume that every entry in single scene or additive scenes
             * are valid so long as SingleScene or AdditiveScenes are not null. */

            //Create moved objects scene. It will probably be used eventually. If not, no harm either way.
            if (string.IsNullOrEmpty(_movedObjectsScene.name))
                _movedObjectsScene = SceneManager.CreateScene("MovedObjectsHolder");
            //Scenes processed by a client during this method.
            HashSet<SceneReferenceData> clientProcessedScenes = new HashSet<SceneReferenceData>();
            //SceneDatas generated for single and additive scenes within this SceneQueueData which are already loaded, or have been.
            SceneReferenceData singleSceneReferenceData = new SceneReferenceData();
            List<SceneReferenceData> additiveSceneReferenceDatas = new List<SceneReferenceData>();
            //Single scene which is loaded, or is to be loaded. Will contain a valid scene if a single scene is specified.
            Scene singleScene = new Scene();
            //True if a connections load and is client only.
            bool connectionsAndClientOnly = (sqd.ScopeType == SceneScopeTypes.Connections && !NetworkServer.active);
            //True if a single scene is specified, whether it needs to be loaded or not.
            bool singleSceneSpecified = (sqd.SingleScene != null && !string.IsNullOrEmpty(sqd.SingleScene.SceneReferenceData.Name));

            /* Scene queue data scenes.
            * All scenes in the scene queue data whether they will be loaded or not. */
            List<string> requestedLoadScenes = new List<string>();
            if (sqd.SingleScene != null)
                requestedLoadScenes.Add(sqd.SingleScene.SceneReferenceData.Name);
            if (sqd.AdditiveScenes != null)
            {
                for (int i = 0; i < sqd.AdditiveScenes.SceneReferenceDatas.Length; i++)
                    requestedLoadScenes.Add(sqd.AdditiveScenes.SceneReferenceDatas[i].Name);
            }

            /* Add to client processed scenes. */
            if (!sqd.AsServer)
            {
                /* Add all scenes to client processed scenes, wether loaded or not.
                 * This is so client can tell the server they have those scenes ready
                 * afterwards, and server will update observers. */
                if (sqd.SingleScene != null)
                    clientProcessedScenes.Add(sqd.SingleScene.SceneReferenceData);

                if (sqd.AdditiveScenes != null)
                {
                    for (int i = 0; i < sqd.AdditiveScenes.SceneReferenceDatas.Length; i++)
                        clientProcessedScenes.Add(sqd.AdditiveScenes.SceneReferenceDatas[i]);
                }
            }

            /* Set networked scenes.
             * If server, and networked scope. */
            if (sqd.AsServer && sqd.ScopeType == SceneScopeTypes.Networked)
            {
                //If single scene specified then reset networked scenes.
                if (singleSceneSpecified)
                    _networkedScenes = new NetworkedScenesData();

                if (sqd.SingleScene != null)
                    _networkedScenes.Single = sqd.SingleScene.SceneReferenceData.Name;
                if (sqd.AdditiveScenes != null)
                {
                    List<string> newNetworkedScenes = _networkedScenes.Additive.ToList();
                    foreach (SceneReferenceData item in sqd.AdditiveScenes.SceneReferenceDatas)
                    {
                        /* Add to additive only if it doesn't already exist.
                         * This is because the same scene cannot be loaded
                         * twice as a networked scene, though it can if loading for a connection. */
                        if (!_networkedScenes.Additive.Contains(item.Name))
                            newNetworkedScenes.Add(item.Name);

                        _networkedScenes.Additive = newNetworkedScenes.ToArray();
                    }
                }

                //Update queue data.
                sqd.NetworkedScenes = _networkedScenes;
            }

            /* LoadableScenes and SceneReferenceDatas.
            /* Will contain scenes which may be loaded.
             * Scenes might not be added to loadableScenes
             * if for example loadOnlyUnloaded is true and
             * the scene is already loaded. */
            List<LoadableScene> loadableScenes = new List<LoadableScene>();
            bool loadSingleScene = false;
            //Add single.
            if (sqd.SingleScene != null)
            {
                loadSingleScene = CanLoadScene(sqd.SingleScene.SceneReferenceData, sqd.LoadOnlyUnloaded, sqd.AsServer);
                //If can load.
                if (loadSingleScene)
                {
                    loadableScenes.Add(new LoadableScene(sqd.SingleScene.SceneReferenceData.Name, LoadSceneMode.Single));
                }
                //If cannot load, see if it already exist, and if so add to server scene datas.
                else
                {
                    singleScene = TryAddToServerSceneDatas(sqd.SingleScene.SceneReferenceData.Name, ref singleSceneReferenceData);
                }
            }
            //Add additives.
            if (sqd.AdditiveScenes != null)
            {
                foreach (SceneReferenceData sceneData in sqd.AdditiveScenes.SceneReferenceDatas)
                {
                    if (CanLoadScene(sceneData, sqd.LoadOnlyUnloaded, sqd.AsServer))
                        loadableScenes.Add(new LoadableScene(sceneData.Name, LoadSceneMode.Additive));
                    else
                        TryAddToServerSceneDatas(sceneData.Name, ref additiveSceneReferenceDatas);
                }
            }

            /* Resetting SceneConnections.
             * Run when a single scene is being loaded, and when running
             * method as server. */
            if (loadSingleScene && sqd.AsServer)
            {
                /* If networked single load then clear all scene connections
                 * and networked scenes. They will be rebuilt shortly after. */
                if (sqd.ScopeType == SceneScopeTypes.Networked)
                    SceneConnections.Clear();
                /* If only certain connections then remove connections
                 * from all scenes. They will be placed into new scenes
                 * once they confirm the scenes have loaded on their end. */
                else if (sqd.ScopeType == SceneScopeTypes.Connections)
                {
                    //Remove connections from all scenes.
                    foreach (KeyValuePair<Scene, HashSet<NetworkConnection>> item in SceneConnections)
                    {
                        for (int i = 0; i < sqd.Connections.Length; i++)
                            item.Value.Remove(sqd.Connections[i]);
                    }
                }

                /* Shouldn't have to refresh checker.
                 * Since the scene is being destroyed, the checkers
                 * are also being destroyed for those scenes. In result
                 * they don't need to be rebuilt. vvvvvvvv */
                ///* Refresh scene checkers.
                // * All scene checkers must be refreshed. This ensures
                // * the new scene checkers are refreshed, as well
                // * the ones which the objects were removed from. */
                //foreach (FlexSceneChecker fsc in _sceneCheckers)
                //    fsc.RebuildObservers();
            }

            /* Move identities to holder scene to preserve them. 
             * Required if a valid single scene exist. Cannot rely on
             * loadSingleScene since it is only true if the single scene
             * must be loaded, which may be false if it's already loaded on
             * the server. */
            if (singleSceneSpecified)
            {
                /* Objects can only be moved if client only, or
                 * if running as server. */
                if ((NetworkClient.active && !NetworkServer.active) || sqd.AsServer)
                {
                    /* If load is for connections, and is as client only.
                     * All network identities must be preserved during the scene change.
                     * This is because Mirror will not respawn them if they are destroyed
                     * on client. They must be destroyed on server for a respawn to queue,
                     * but since the load is happening only for certain clients rather
                     * than the server, this is not the case. */
                    if (sqd.ScopeType == SceneScopeTypes.Connections && !NetworkServer.active)
                    {
                        /* Don't change which scenes these belong in since they
                         * are just being moved to preserve the object. */
                        foreach (var item in NetworkIdentity.spawned)
                            SceneManager.MoveGameObjectToScene(item.Value.gameObject, _movedObjectsScene);
                    }
                    //Any other condition requires moving only the network identities passed in.
                    else
                    {
                        if (sqd.SingleScene.MovedNetworkIdentities != null)
                        {
                            foreach (NetworkIdentity ni in sqd.SingleScene.MovedNetworkIdentities)
                                SceneManager.MoveGameObjectToScene(ni.gameObject, _movedObjectsScene);
                        }
                    }
                }
            }


            /* Scene unloading.
             * Requires a single scene to be specified. Only run if
             * networked scene, or scene change is for connections and are client only. */
            bool canTryUnloads = (singleSceneSpecified && (sqd.ScopeType == SceneScopeTypes.Networked || connectionsAndClientOnly));
            /* Unload all scenes (except moved objects scene). */
            /* Make a list for scenes which will be unloaded rather
            * than unload during the iteration. This is to prevent a
            * collection has changed error. 
            *
            * unloadableScenes is created so that if either unloadableScenes
            * or loadableScenes has value, the OnLoadStart event knows to dispatch. */
            List<Scene> unloadableScenes = new List<Scene>();
            if (canTryUnloads)
            {

                //Unload all other scenes.
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene c = SceneManager.GetSceneAt(i);

                    //True if the scene being checked to unload is in scene queue data.
                    bool inSceneQueueData = requestedLoadScenes.Contains(c.name);
                    /* canUnload becomes true when the scene is
                     * not in the scene queue data, and when it passes
                     * CanUnloadScene conditions. */
                    bool canUnload = (!inSceneQueueData && CanUnloadScene(c.name, sqd.NetworkedScenes));
                    //If not scene being changed to and not the object holder scene.
                    if (c.name != _movedObjectsScene.name && canUnload)
                        unloadableScenes.Add(c);
                }
            }

            /* Start event. */
            if (unloadableScenes.Count > 0 || loadableScenes.Count > 0)
                InvokeOnSceneLoadStart(sqd);

            //If can try unloads, now unload any unloadable scenes.
            if (canTryUnloads)
            {
                for (int i = 0; i < unloadableScenes.Count; i++)
                {
                    //Unload one at a time.
                    AsyncOperation async = SceneManager.UnloadSceneAsync(unloadableScenes[i]);
                    while (!async.isDone)
                        yield return null;
                }
            }

            //Scenes which have been loaded.
            List<Scene> loadedScenes = new List<Scene>();
            /* Scene loading.
            /* Use additive to not thread lock server. */
            for (int i = 0; i < loadableScenes.Count; i++)
            {
                //Start load async and wait for it to finish.
                LoadSceneParameters loadSceneParameters = new LoadSceneParameters()
                {
                    loadSceneMode = LoadSceneMode.Additive,
                    localPhysicsMode = sqd.LocalPhysics
                };
                AsyncOperation loadAsync = SceneManager.LoadSceneAsync(loadableScenes[i].SceneName, loadSceneParameters);
                while (!loadAsync.isDone)
                {
                    /* How much percentage each scene load can be worth
                     * at maximum completion. EG: if there are two scenes
                     * 1f / 2f is 0.5f. */
                    float maximumIndexWorth = (1f / (float)loadableScenes.Count);
                    /* Total percent will be how much percentage is complete
                     * in total. Initialize it with a value based on how many
                     * scenes are already fully loaded. */
                    float totalPercent = (i * maximumIndexWorth);
                    //Add this scenes progress onto total percent.
                    totalPercent += Mathf.Lerp(0f, maximumIndexWorth, loadAsync.progress);

                    //Dispatch with total percent.
                    InvokeOnScenePercentChange(sqd, totalPercent);
                    yield return null;
                }

                //After loaded, add to loaded scenes and datas.
                Scene lastLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
                SceneReferenceData sd = new SceneReferenceData()
                {
                    Handle = lastLoadedScene.handle,
                    Name = lastLoadedScene.name
                };
                //Add to loaded scenes.
                loadedScenes.Add(lastLoadedScene);

                /* Scene references */
                if (loadableScenes[i].LoadMode == LoadSceneMode.Single)
                {
                    singleSceneReferenceData = sd;
                    singleScene = lastLoadedScene;
                }
                else if (loadableScenes[i].LoadMode == LoadSceneMode.Additive)
                {
                    additiveSceneReferenceDatas.Add(sd);
                }
            }
            //When all scenes are loaded invoke with 100% done.
            InvokeOnScenePercentChange(sqd, 1f);

            /* Move identities to new single scene.
             * Move if server, or scope is Networked.
             * Make single scene active scene. */
            if (singleSceneSpecified)
            {
                /* Move identities.
                * Only required if server, or client and networked load. 
                * Server needs to move identities to new scene to ensure physic will
                * work if scenes are using local physics. Clients needs to keep objects
                * in the moved objects scene so that they are not destroyed when loading
                * a single scene. If the client were to destroy the objects locally but
                * they weren't on the server, then errors will throw since Mirror won't have
                * an object to send messages to.
                * 
                * It's okay to move regardless if client or not if the scene change type is
                * networked because the server will also be unloading the old scenes. Where-as
                * with connections only the server won't neccesarily unload the old scenes, so
                * the objects should persist on client as well. */
                if (sqd.AsServer || sqd.ScopeType == SceneScopeTypes.Networked)
                {
                    for (int i = 0; i < sqd.SingleScene.MovedNetworkIdentities.Length; i++)
                    {
                        /* The identities were already cleaned but this is just incase something happened
                         * to them while scenes were loading. */
                        foreach (NetworkIdentity ni in sqd.SingleScene.MovedNetworkIdentities)
                        {
                            if (ni != null && ni.netId != 0)
                            {
                                SceneManager.MoveGameObjectToScene(ni.gameObject, singleScene);
                                if (sqd.AsServer)
                                    ReplaceWithSceneInternal(singleScene, new NetworkConnection[] { ni.connectionToClient });
                            }
                        }
                    }
                }

                /* Set active scene.
                 * If networked, since all clients will be changing.
                 * Or if connectionsAndClientOnly. 
                 * 
                 * Cannot change active scene if client host because new objects will spawn
                 * into the single scene intended for only certain connections; this will break observers. */
                if (sqd.ScopeType == SceneScopeTypes.Networked || connectionsAndClientOnly)
                    SceneManager.SetActiveScene(singleScene);
            }

            /* Completion messages.
            * If running as server. */
            if (sqd.AsServer)
            {
                if (sqd.SingleScene != null)
                    sqd.SingleScene.SceneReferenceData = singleSceneReferenceData;
                if (sqd.AdditiveScenes != null)
                    sqd.AdditiveScenes.SceneReferenceDatas = additiveSceneReferenceDatas.ToArray();

                /* Make SceneQueueData serializable again.
                 * Data may have been altered when removing invalid entries. */
                sqd.MakeSerializable();
                //Tell clients to load same scenes.
                LoadScenesMessage msg = new LoadScenesMessage()
                {
                    SceneQueueData = sqd
                };

                //If networked scope then send to all.
                if (sqd.ScopeType == SceneScopeTypes.Networked)
                {
                    NetworkServer.SendToAll(msg);
                }
                //If connections scope then only send to connections.
                else if (sqd.ScopeType == SceneScopeTypes.Connections)
                {
                    if (sqd.Connections != null)
                    {
                        for (int i = 0; i < sqd.Connections.Length; i++)
                        {
                            if (sqd.Connections[i] != null)
                                sqd.Connections[i].Send(msg);
                        }
                    }
                }
            }
            /* If running as client then send a message
             * to the server to tell them the scene was loaded.
             * This allows the server to add the client
             * to the scene for checkers. */
            else if (!sqd.AsServer)
            {
                ClientScenesLoadedMessage msg = new ClientScenesLoadedMessage()
                {
                    SceneDatas = clientProcessedScenes.ToArray()
                };
                NetworkClient.Send(msg);
            }

            InvokeOnSceneLoadEnd(sqd, requestedLoadScenes, loadedScenes);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="conns"></param>
        [Server]
        private void ReplaceWithSceneInternal(Scene scene, NetworkConnection[] conns)
        {
            if (string.IsNullOrEmpty(scene.name) || conns == null || conns.Length == 0)
                return;

            //Remove from all scenes.
            for (int i = 0; i < conns.Length; i++)
            {
                foreach (var item in SceneConnections)
                    item.Value.Remove(conns[i]);
            }

            AddToScene(scene, conns);
        }


        /// <summary>
        /// Tries to find a scene by name and if found adds it to the specified scene data.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="sceneReferenceDatas"></param>
        /// <returns>Scene added if found.</returns>
        private Scene TryAddToServerSceneDatas(string sceneName, ref List<SceneReferenceData> sceneReferenceDatas)
        {
            Scene s = SceneManager.GetSceneByName(sceneName);
            if (!string.IsNullOrEmpty(s.name))
            {
                SceneReferenceData sd = new SceneReferenceData()
                {
                    Handle = s.handle,
                    Name = s.name
                };
                sceneReferenceDatas.Add(sd);

                return s;
            }

            return new Scene();
        }
        /// <summary>
        /// Tries to find a scene by name and if found sets it to the specified scene data.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="sceneReferenceData"></param>
        /// <returns>Scene added if found.</returns>
        private Scene TryAddToServerSceneDatas(string sceneName, ref SceneReferenceData sceneReferenceData)
        {
            Scene s = SceneManager.GetSceneByName(sceneName);
            if (!string.IsNullOrEmpty(s.name))
            {
                SceneReferenceData sd = new SceneReferenceData()
                {
                    Handle = s.handle,
                    Name = s.name
                };
                sceneReferenceData = sd;

                return s;
            }

            return new Scene();
        }


        /// <summary>
        /// Received on client when connection scenes must be loaded.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        [Client]
        private void OnLoadScenes(NetworkConnection conn, LoadScenesMessage msg)
        {
            LoadSceneQueueData sqd = msg.SceneQueueData;
            //Clients will always only load scenes once.
            sqd.LoadOnlyUnloaded = true;
            LoadScenesInternal(sqd.ScopeType, null, sqd.SingleScene, sqd.AdditiveScenes, sqd.LoadOnlyUnloaded, LocalPhysicsMode.None, sqd.NetworkedScenes, false);
        }
        #endregion

        #region UnloadScenes.
        /// <summary>
        /// Unloads additive scenes for all connections.
        /// </summary>
        /// <param name="additiveScenes">Scenes to unload by string lookup.</param>
        [Server]
        public static void UnloadNetworkedScenes(string[] additiveScenes)
        {
            AdditiveScenesData asd = new AdditiveScenesData(additiveScenes);
            UnloadNetworkedScenes(asd);
        }
        /// <summary>
        /// Unloads additive scenes for all connections.
        /// </summary>
        /// <param name="additiveScenes">Scenes to unload by scene references.</param>
        [Server]
        public static void UnloadNetworkedScenes(AdditiveScenesData additiveScenes)
        {
            _instance.UnloadScenesInternal(SceneScopeTypes.Networked, null, additiveScenes, _instance._networkedScenes, true);
        }
        /// <summary>
        /// Unloads scenes on server and tells connections to unload them as well. Other connections will not unload this scene.
        /// </summary>
        /// <param name="conns">Connections to unload scenes for.</param>
        /// <param name="additiveScenes">Scenes to unload by string lookup.</param>
        [Server]
        public static void UnloadConnectionScenes(NetworkConnection[] conns, string[] additiveScenes)
        {
            AdditiveScenesData asd = new AdditiveScenesData(additiveScenes);
            UnloadConnectionScenes(conns, asd);
        }
        /// <summary>
        /// Unloads scenes on server and tells connections to unload them as well. Other connections will not unload this scene.
        /// </summary>
        /// <param name="conns">Connections to unload scenes for.</param>
        /// <param name="additiveScenes">Scenes to unload by scene references.</param>
        [Server]
        public static void UnloadConnectionScenes(NetworkConnection[] conns, AdditiveScenesData additiveScenes)
        {
            _instance.UnloadScenesInternal(SceneScopeTypes.Connections, conns, additiveScenes, _instance._networkedScenes, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="conns"></param>
        /// <param name="additiveScenes"></param>
        /// <param name="asServer"></param>
        private void UnloadScenesInternal(SceneScopeTypes scope, NetworkConnection[] conns, AdditiveScenesData additiveScenes, NetworkedScenesData networkedScenes, bool asServer)
        {
            //Add to scene queue data.        
            _queuedSceneOperations.Add(new UnloadSceneQueueData(scope, conns, additiveScenes, networkedScenes, asServer));
            /* If only one entry then scene operations are not currently in progress.
             * Should there be more than one entry then scene operations are already 
             * occuring. The coroutine will automatically load in order. */
            if (_queuedSceneOperations.Count == 1)
                StartCoroutine(__ProcessSceneQueue());
        }

        /// <summary>
        /// Loads scenes within QueuedSceneLoads.
        /// </summary>
        /// <returns></returns>
        private IEnumerator __UnloadScenes()
        {
            UnloadSceneQueueData sqd = _queuedSceneOperations[0] as UnloadSceneQueueData;
            RemoveInvalidSceneQueueData(ref sqd);
            /* No additive scenes to unload. */
            if (sqd.AdditiveScenes == null)
                yield break;

            /* It's safe to assume that every entry in additive scenes
             * are valid so long as AdditiveScenes are not null. */

            /* Remove from networked scenes.
            * If server and scope is networked. 
            * All passed in scenes should be removed from networked
            * regardless of if they're valid or not. If they are invalid,
            * then they shouldn't be in networked to begin with. */
            if (sqd.AsServer && sqd.ScopeType == SceneScopeTypes.Networked)
            {
                List<string> newNetworkedScenes = _networkedScenes.Additive.ToList();
                //Remove unloaded from networked scenes.
                foreach (SceneReferenceData item in sqd.AdditiveScenes.SceneReferenceDatas)
                    newNetworkedScenes.Remove(item.Name);
                _networkedScenes.Additive = newNetworkedScenes.ToArray();

                //Update queue data.
                sqd.NetworkedScenes = _networkedScenes;
            }

            /* Build unloadable scenes collection. */
            List<Scene> unloadableScenes = new List<Scene>();
            foreach (SceneReferenceData item in sqd.AdditiveScenes.SceneReferenceDatas)
            {
                /*If running as client but also the server then don't add scenes
                * as they already unloaded on the server. */
                if (!sqd.AsServer && NetworkServer.active)
                    continue;

                Scene s;
                /* If the handle exist and as server
                 * then unload using the handle. Otherwise
                 * unload using the name. Handles are used to
                 * unload scenes with the same name, which would
                 * only occur on the server since it can spawn multiple instances
                 * of the same scene. Client will always only have
                 * one copy of each scene so it must get the scene
                 * by name. */
                if (item.Handle != 0 && sqd.AsServer)
                    s = GetSceneByHandle(item.Handle);
                else
                    s = SceneManager.GetSceneByName(item.Name);

                if (CanUnloadScene(s, sqd.NetworkedScenes))
                    unloadableScenes.Add(s);
            }


            //If there are scenes to unload.
            if (unloadableScenes.Count > 0)
            {
                /* Update visibilities. */
                ClientVisibilityChangeEventArgs cvc = new ClientVisibilityChangeEventArgs(unloadableScenes, sqd.Connections, false);
                OnClientVisibilityChangeStart?.Invoke(cvc);
                //Remove connections from every scene to unload.
                for (int i = 0; i < unloadableScenes.Count; i++)
                    RemoveFromScene(unloadableScenes[i], sqd.Connections);
                OnClientVisibilityChangeEnd?.Invoke(cvc);

                /* Make sure scenes can be unloaded if for connection.
                 * Scenes cannot be unloaded on the server if other clients
                 * still exist in the scene. */
                if (sqd.AsServer && sqd.ScopeType == SceneScopeTypes.Connections)
                {
                    //Remove from unloadables if the scene still contains connections.
                    for (int i = 0; i < unloadableScenes.Count; i++)
                    {
                        HashSet<NetworkConnection> conns;
                        if (SceneConnections.TryGetValue(unloadableScenes[i], out conns))
                        {
                            if (conns.Count > 0)
                            {
                                unloadableScenes.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                }

                /* If there are still scenes to unload after connections pass.
                 * There may not be scenes to unload as if another connection still
                 * exist in the unloadable scenes, then they cannot be unloaded. */
                if (unloadableScenes.Count > 0)
                {
                    InvokeOnSceneUnloadStart(sqd);

                    /* Remove each scene key from SceneConnections.
                     * There is no reason to update observers because
                     * the scene will be unloaded, which will remove
                     * the observer entirely. */
                    for (int i = 0; i < unloadableScenes.Count; i++)
                        SceneConnections.Remove(unloadableScenes[i]);

                    /* Unload scenes.
                    /* Use additive to not thread lock server. */
                    foreach (Scene s in unloadableScenes)
                    {
                        AsyncOperation async = SceneManager.UnloadSceneAsync(s);
                        while (!async.isDone)
                            yield return null;
                    }
                }
            }

            /* If running as server. */
            if (sqd.AsServer)
            {
                /* Make SceneQueueData serializable again.
                 * Data may have been altered when removing invalid entries. */
                sqd.MakeSerializable();
                //Tell clients to unload same scenes.
                UnloadScenesMessage msg = new UnloadScenesMessage()
                {
                    SceneQueueData = sqd
                };
                //If connections scope.
                if (sqd.ScopeType == SceneScopeTypes.Networked)
                {
                    NetworkServer.SendToAll(msg);
                }
                //Networked scope.
                else if (sqd.ScopeType == SceneScopeTypes.Connections)
                {
                    if (sqd.Connections != null)
                    {
                        for (int i = 0; i < sqd.Connections.Length; i++)
                        {
                            if (sqd.Connections[i] != null)
                                sqd.Connections[i].Send(msg);
                        }
                    }
                }
            }

            InvokeOnSceneUnloadEnd(sqd);
        }
        /// <summary>
        /// Received on clients when networked scenes must be unloaded.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="msg"></param>
        private void OnUnloadScenes(NetworkConnection conn, UnloadScenesMessage msg)
        {
            UnloadSceneQueueData sqd = msg.SceneQueueData;
            UnloadScenesInternal(sqd.ScopeType, sqd.Connections, sqd.AdditiveScenes, sqd.NetworkedScenes, false);
        }
        #endregion

        #region Add scene checkers.
        /// <summary>
        /// Adds a FlexSceneChecker to checkers.
        /// </summary>
        /// <param name="checker"></param>
        public static void AddSceneChecker(FlexSceneChecker checker)
        {
            _instance._sceneCheckers.Add(checker);
        }
        /// <summary>
        /// Sets a connection as being in a scene and updates FlexSceneCheckers.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="conn"></param>
        [Server]
        public static void AddToScene(Scene scene, NetworkConnection conn)
        {
            _instance.AddToSceneInternal(scene, new NetworkConnection[] { conn });
        }
        /// <summary>
        /// Sets connections as being in a scene and updates FlexSceneCheckers.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="conns"></param>
        [Server]
        public static void AddToScene(Scene scene, NetworkConnection[] conns)
        {
            _instance.AddToSceneInternal(scene, conns);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="conns"></param>
        [Server]
        private void AddToSceneInternal(Scene scene, NetworkConnection[] conns)
        {
            if (string.IsNullOrEmpty(scene.name) || conns == null || conns.Length == 0)
                return;

            HashSet<NetworkConnection> hs;
            /* If the scene hasn't been added to the collection
             * yet then add it with an empty hashset. The hashet
             * will be populated below. */
            if (!SceneConnections.TryGetValue(scene, out hs))
            {
                hs = new HashSet<NetworkConnection>();
                SceneConnections[scene] = hs;
            }

            bool added = false;
            //Go through each connection and add to hashset.
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    Debug.LogWarning("NetworkConnection is null.");
                    continue;
                }

                /* Check if this object has a scene checker, and if it does then
                * update the scene checker to include the scene it's being added
                * to. */
                if (conns[i].identity != null && conns[i].identity.GetComponent<FlexSceneChecker>() is FlexSceneChecker fsc)
                {
                    //Also set added if scene checker was updated, so that all scene checkers refresh.
                    added = true;
                    fsc.AddedToScene(scene);
                }

                bool r = hs.Add(conns[i]);
                if (r)
                    added = true;
            }

            /* If any connections were modified from scenes. */
            if (added)
            {
                foreach (FlexSceneChecker item in _sceneCheckers)
                    item.RebuildObservers();
            }
        }
        #endregion

        #region Remove scene checkers.
        /// <summary>
        /// Removes a FlexSceneChecker from checkers.
        /// </summary>
        /// <param name="checker"></param>
        public static void RemoveSceneChecker(FlexSceneChecker checker)
        {
            _instance._sceneCheckers.Remove(checker);
        }
        /// <summary>
        /// Unsets a connection as being in a scene and updates FlexSceneCheckers.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="conn"></param>
        [Server]
        public static void RemoveFromScene(Scene scene, NetworkConnection conn)
        {
            _instance.RemoveFromSceneInternal(scene, new NetworkConnection[] { conn });
        }
        /// <summary>
        /// Unsets connections as being in a scene and updates FlexSceneCheckers.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="conns"></param>
        [Server]
        public static void RemoveFromScene(Scene scene, NetworkConnection[] conns)
        {
            _instance.RemoveFromSceneInternal(scene, conns);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="conns"></param>
        [Server]
        private void RemoveFromSceneInternal(Scene scene, NetworkConnection[] conns)
        {
            if (string.IsNullOrEmpty(scene.name) || conns == null || conns.Length == 0)
                return;

            HashSet<NetworkConnection> hs;
            /* If sceneName is not in the collection then nothing
             * can be removed as the hashset does not exist. */
            if (!SceneConnections.TryGetValue(scene, out hs))
                return;

            bool removed = false;
            //Go through each connection and remove from hashset.
            for (int i = 0; i < conns.Length; i++)
            {
                if (conns[i] == null)
                {
                    Debug.LogWarning("NetworkConnection is null or unset.");
                    continue;
                }

                /* Check if this object has a scene checker, and if it does then
                 * update the scene checker to remove the scene it's being removed
                 * from. */
                if (conns[i].identity != null && conns[i].identity.GetComponent<FlexSceneChecker>() is FlexSceneChecker fsc)
                {
                    //Also set removed so that scene checkers update.
                    removed = true;
                    fsc.RemovedFromScene(scene);
                }

                bool r = hs.Remove(conns[i]);
                if (r)
                    removed = true;
            }

            /* If any connections were modified from scenes. */
            if (removed)
            {
                foreach (FlexSceneChecker item in _sceneCheckers)
                    item.RebuildObservers();
            }
        }
        #endregion

        #region RemoveInvalidScenes.
        /// <summary>
        /// Removes invalid scene entries from a SceneQueueData.
        /// </summary>
        /// <param name="sceneDatas"></param>
        private void RemoveInvalidSceneQueueData(ref LoadSceneQueueData sqd)
        {
            //Check single scene.
            //If scene name is invalid.
            if (string.IsNullOrEmpty(sqd.SingleScene.SceneReferenceData.Name) ||
                //Loading for connection but already a single networked scene.
                (sqd.ScopeType == SceneScopeTypes.Connections && IsNetworkedScene(sqd.SingleScene.SceneReferenceData.Name, _networkedScenes))
                )
                sqd.SingleScene = null;

            //Check additive scenes.
            if (sqd.AdditiveScenes != null)
            {
                //Make all scene names into a list for easy removal.
                List<SceneReferenceData> listSceneReferenceDatas = sqd.AdditiveScenes.SceneReferenceDatas.ToList();
                for (int i = 0; i < listSceneReferenceDatas.Count; i++)
                {
                    //Scene name is null or empty.
                    if (string.IsNullOrEmpty(listSceneReferenceDatas[i].Name))
                    {
                        listSceneReferenceDatas.RemoveAt(i);
                        i--;
                    }
                }
                //Set back to array.
                sqd.AdditiveScenes.SceneReferenceDatas = listSceneReferenceDatas.ToArray();

                //If additive scene names is null or has no length then nullify additive scenes.
                if (sqd.AdditiveScenes.SceneReferenceDatas == null || sqd.AdditiveScenes.SceneReferenceDatas.Length == 0)
                    sqd.AdditiveScenes = null;
            }

            //Connections.
            if (sqd.Connections != null)
            {
                List<NetworkConnection> listConnections = sqd.Connections.ToList();
                for (int i = 0; i < listConnections.Count; i++)
                {
                    if (listConnections[i] == null || listConnections[i].connectionId == 0)
                    {
                        listConnections.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        /// <summary>
        /// Removes invalid scene entries from a SceneQueueData.
        /// </summary>
        /// <param name="sceneDatas"></param>
        private void RemoveInvalidSceneQueueData(ref UnloadSceneQueueData sqd)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            //Check additive scenes.
            if (sqd.AdditiveScenes != null)
            {
                NetworkedScenesData networkedScenes = (NetworkServer.active) ? _networkedScenes : sqd.NetworkedScenes;
                //Make all scene names into a list for easy removal.
                List<SceneReferenceData> listSceneNames = sqd.AdditiveScenes.SceneReferenceDatas.ToList();
                for (int i = 0; i < listSceneNames.Count; i++)
                {
                    //If scene name is null or zero length/
                    if (string.IsNullOrEmpty(listSceneNames[i].Name) ||
                        //Or if scene name is active scene.
                        (activeScene != null && listSceneNames[i].Name == activeScene.name) ||
                        //If unloading as connection but scene is networked.
                        (sqd.ScopeType == SceneScopeTypes.Connections && IsNetworkedScene(listSceneNames[i].Name, networkedScenes))
                        )
                    {
                        listSceneNames.RemoveAt(i);
                        i--;
                    }
                }
                //Set back to array.
                sqd.AdditiveScenes.SceneReferenceDatas = listSceneNames.ToArray();

                //If additive scene names is null or has no length then nullify additive scenes.
                if (sqd.AdditiveScenes.SceneReferenceDatas == null || sqd.AdditiveScenes.SceneReferenceDatas.Length == 0)
                    sqd.AdditiveScenes = null;
            }
        }
        #endregion

        #region Can Load/Unload Scene.
        /// <summary>
        /// Returns if a scene name can be loaded.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadOnlyUnloaded"></param>
        /// <returns></returns>
        private bool CanLoadScene(SceneReferenceData sceneReferenceData, bool loadOnlyUnloaded, bool asServer)
        {
            /* When a handle is specified a scene can only be loaded if the handle does not exist.
             * This is regardless of loadOnlyUnloaded value. This is also only true for the server, as
             * only the server actually utilizies/manages handles. This feature exist so users may stack scenes
             * by setting loadOnlyUnloaded false, while also passing in a scene reference which to add a connection
             * to.
             * 
             * For example: if scene stacking is enabled(so, !loadOnlyUnloaded), and a player is the first to join Blue scene. Let's assume
             * the handle for that spawned scene becomes -10. Later, the server wants to add another player to the same
             * scene. They would generate the load scene data, passing in the handle of -10 for the scene to load. The
             * loader will then check if a scene is loaded by that handle, and if so add the player to that scene rather than
             * load an entirely new scene. If a scene does not exist then a new scene will be made. */
            if (asServer && sceneReferenceData.Handle != 0)
            {
                if (!string.IsNullOrEmpty(GetSceneByHandle(sceneReferenceData.Handle).name))
                    return false;
            }

            return CanLoadScene(sceneReferenceData.Name, loadOnlyUnloaded);
        }
        /// <summary>
        /// Returns if a scene name can be loaded.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="loadOnlyUnloaded"></param>
        /// <returns></returns>
        private bool CanLoadScene(string sceneName, bool loadOnlyUnloaded)
        {
            if (string.IsNullOrEmpty(sceneName))
                return false;

            if (!loadOnlyUnloaded || (loadOnlyUnloaded && !IsSceneLoaded(sceneName)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Returns if a scene can be unloaded.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="scopeType"></param>
        /// <returns></returns>
        private bool CanUnloadScene(string sceneName, NetworkedScenesData networkedScenes)
        {
            //Not loaded.
            if (!IsSceneLoaded(sceneName))
                return false;

            /* Cannot unload networked scenes.
             * If a scene should be unloaded, that is networked,
             * then it must be removed from the networked scenes
             * collection first. */
            if (IsNetworkedScene(sceneName, networkedScenes))
                return false;

            return true;
        }

        /// <summary>
        /// Returns if a scene can be unloaded.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="scopeType"></param>
        /// <returns></returns>
        private bool CanUnloadScene(Scene scene, NetworkedScenesData networkedScenes)
        {
            return CanUnloadScene(scene.name, networkedScenes);
        }
        #endregion

        #region Helpers.
        /// <summary>
        /// Returns if a sceneName is a networked scene.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private bool IsNetworkedScene(string sceneName, NetworkedScenesData networkedScenes)
        {
            if (string.IsNullOrEmpty(sceneName))
                return false;

            //Matches single sene.
            if (networkedScenes.Single != null && sceneName == networkedScenes.Single)
                return true;

            //Matches at least one additive.
            if (networkedScenes.Additive != null)
            {
                if (networkedScenes.Additive.Contains(sceneName))
                    return true;
            }

            //Fall through, no matches.
            return false;
        }
        /// <summary>
        /// Returns if a scene is loaded.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        private bool IsSceneLoaded(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return false;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a scene by handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static Scene GetSceneByHandle(int handle)
        {
            return _instance.GetSceneByHandleInternal(handle);
        }
        /// <summary>
        /// Returns a scene by handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        private Scene GetSceneByHandleInternal(int handle)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.handle == handle)
                    return s;
            }

            //Fall through, not found.
            return new Scene();
        }
        #endregion

        #region Unused.
        ///// <summary>
        ///// Returns if it's possible to attempt a MoveToScene.
        ///// </summary>
        ///// <param name="scene"></param>
        ///// <param name="netIdentities"></param>
        ///// <returns></returns>
        //private bool ValidateMoveNetworkIdentity(string newScene, NetworkIdentity[] netIdentities)
        //{
        //    //Network Identity null or unset.
        //    if (netIdentities == null || netIdentities.Length == 0)
        //    {
        //        Debug.LogError("NetworkIdentities is either null or of zero length.");
        //        return false;
        //    }

        //    //Make sure scene is loaded.
        //    Scene scene = GetSceneByName(newScene);
        //    if (scene.path == null)
        //        return false;

        //    //First make sure the scene is loaded.
        //    bool newSceneLoaded = false;
        //    for (int i = 0; i < SceneManager.sceneCount; i++)
        //    {
        //        //Found scene.
        //        if (SceneManager.GetSceneAt(i).name == newScene)
        //        {
        //            newSceneLoaded = true;
        //            break;
        //        }
        //    }
        //    /* If newScene isn't loaded then the networkidentity
        //    * cannot be moved. */
        //    if (!newSceneLoaded)
        //    {
        //        Debug.LogError("Scene " + newScene + " is not loaded.");
        //        return false;
        //    }

        //    //Fall through. If here all checks passed.
        //    return true;
        //}

        ///// <summary>
        ///// Moves a NetworkIdentity to a new scene on server and clients.
        ///// </summary>
        ///// <param name="newScene"></param>
        ///// <param name="netIdentities"></param>
        ///// <param name="reloadScene"></param>
        //[Server]
        //public void MoveNetworkIdentities(string newScene, NetworkIdentity[] netIdentities, bool broadcastToClients)
        //{
        //    if (!ValidateMoveNetworkIdentity(newScene, netIdentities))
        //        return;

        //    /* Remove the identity from all scenes first.
        //     * A brute force check on scenes is likely faster
        //     * than storing which scene every identity is in. */
        //    foreach (KeyValuePair<string, HashSet<NetworkIdentity>> sceneIds in SceneIdentities)
        //        RemoveFromScene(sceneIds.Key, netIdentities);

        //    //After removed from all scenes add to new scene.
        //    AddToScene(newScene, netIdentities);

        //    Scene scene = GetSceneByName(newScene);
        //    //Move objects to new scene.
        //    for (int i = 0; i < netIdentities.Length; i++)
        //    {
        //        if (netIdentities[i] == null || netIdentities[i].netId == 0)
        //        {
        //            Debug.LogWarning("NetworkIdentity is null or unset.");
        //            continue;
        //        }

        //        SceneManager.MoveGameObjectToScene(netIdentities[i].gameObject, scene);
        //    }

        //}


        ///// <summary>
        ///// Received on clients when they should load a scene and move to it.
        ///// </summary>
        ///// <param name="conn"></param>
        ///// <param name="msg"></param>
        //[ClientCallback]
        //private void OnMoveNetworkIdentity(NetworkConnection conn, MoveNetworkIdentityMessage msg)
        //{
        //    if (!ValidateMoveNetworkIdentity(msg.SceneName, msg.NetworkIdentities))
        //        return;

        //    Scene scene = GetSceneByName(msg.SceneName);
        //    NetworkIdentity[] netIdentities = msg.NetworkIdentities;
        //    //Move objects to new scene.
        //    for (int i = 0; i < netIdentities.Length; i++)
        //    {
        //        if (netIdentities[i] == null || netIdentities[i].netId == 0)
        //        {
        //            Debug.LogWarning("NetworkIdentity is null or unset.");
        //            continue;
        //        }

        //        SceneManager.MoveGameObjectToScene(netIdentities[i].gameObject, scene);
        //    }
        //}

        #endregion

    }


}