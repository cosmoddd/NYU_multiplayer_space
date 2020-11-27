Setup
=====================================
Import all files within FirstGearGames/FlexSceneManager. You may exclude the demos folder.

Create a custom NetworkManager using script templates if you do not already have one. Replace existing network manager script reference.
Within your custom network manager:
    Under override OnServerConnect add the following to the end of the method:
        FlexSceneManager.OnServerConnect(conn);
    Under override OnServerDisconnect add the following to the top of the method:
        FlexSceneManager.OnServerDisconnect(conn);
    Under override OnStartClient add the following to the end of the method:
        FlexSceneManager.ResetInitialLoad();

If you are not using autoCreatePlayer within your network manager then after spawning your player manually, you must also call
    FlexSceneManager.SendPlayerCreated();



Demos
=====================================
Open the "Main" scene within each demo folder. Blue spheres load and unload scenes for connections. Red spheres load and unload scenes for network (all clients).
See demo scripts for examples of loading and unloading.



FlexSceneChecker
=====================================
Add to objects which you only want visible when players have the scene loaded. In most cases this will be on all of your networked objects.
Enable Add To Current Scene if you want the object to be automatically registered to the scene it spawns in. This is typically enabled for scene objects, and sometimes player objects.



Loading and Unloading Data Types
=====================================

SceneReferenceData
	Handle: handle for the scene. This is controlled by the server and has no use for clients.
	Name: name of the scene being loaded.

SingleSceneData: only used when loading scenes.
	SceneReferenceData: scene to load or unload.
	MovedNetworkIdentities: network identities to move to the new scene single scene.

AdditiveScenesData
	SceneReferenceDatas: scenes to load or unload.



Loading and Unloading API: FlexSceneManager.Method()
=====================================

    /// <summary>
    /// Loads scenes which all clients will be synchronized into.
    /// </summary>
    /// <param name="singleScene">Single scene to load. Use null to opt-out of single scene loading.</param>
    /// <param name="additiveScenes">Additive scenes to load. Use null to opt-out of additive scene loading.</param>
    /// <param name="loadOnlyUnloaded">True to only load scenes which are currently not loaded.</param>
    [Server]
    public static void LoadNetworkedScenes(SingleSceneData singleScene, AdditiveScenesData additiveScenes, bool loadOnlyUnloaded = true)

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

    /// <summary>
    /// Unloads additive scenes for all connections.
    /// </summary>
    /// <param name="additiveScenes">Scenes to unload by string lookup.</param>
    [Server]
    public static void UnloadNetworkedScenes(string[] additiveScenes)

    /// <summary>
    /// Unloads additive scenes for all connections.
    /// </summary>
    /// <param name="additiveScenes">Scenes to unload by scene references.</param>
    [Server]
    public static void UnloadNetworkedScenes(AdditiveScenesData additiveScenes)

    /// <summary>
    /// Unloads scenes on server and tells connections to unload them as well. Other connections will not unload this scene.
    /// </summary>
    /// <param name="conns">Connections to unload scenes for.</param>
    /// <param name="additiveScenes">Scenes to unload by string lookup.</param>
    [Server]
    public static void UnloadConnectionScenes(NetworkConnection[] conns, string[] additiveScenes)

    /// <summary>
    /// Unloads scenes on server and tells connections to unload them as well. Other connections will not unload this scene.
    /// </summary>
    /// <param name="conns">Connections to unload scenes for.</param>
    /// <param name="additiveScenes">Scenes to unload by scene references.</param>
    [Server]
    public static void UnloadConnectionScenes(NetworkConnection[] conns, AdditiveScenesData additiveScenes)



Special Use API: FlexSceneManager.Method()
=====================================

If your player object utilizies FlexSceneChecker and you are not using autoCreatePlayer in your network manager then you must call
    SendPlayerCreated()
    after using ClientScene.AddPlayer().




Events: FlexSceneManager
=====================================

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
        /// RawData used by the current scene action.
        /// </summary>
        public readonly LoadSceneQueueData RawData;

    /// <summary>
    /// Dispatched when completion percentage changes while loading a scene. Value is between 0f and 1f, while 1f is 100% done. Can be used for custom progress bars when loading scenes.
    /// </summary>
    public static event Action<LoadScenePercentEventArgs> OnLoadScenePercentChange;
        /// <summary>
        /// RawData used by the current scene action.
        /// </summary>
        public readonly LoadSceneQueueData RawData;
        /// <summary>
        /// Percentage of change completion. 1f is equal to 100 complete.
        /// </summary>
        public readonly float Percent;

    /// <summary>
    /// Dispatched when a scene load ends.
    /// </summary>
    public static event Action<LoadSceneEndEventArgs> OnLoadSceneEnd;
        /// <summary>
        /// RawData used by the current scene action.
        /// </summary>
        public readonly LoadSceneQueueData RawData;
        /// <summary>
        /// Scenes which were loaded.
        /// </summary>
        public readonly Scene[] LoadedScenes;
        /// <summary>
        /// Scenes which were skipped because they were already loaded.
        /// </summary>
        public readonly string[] SkippedSceneNames;

    /// <summary>
    /// Dispatched when a scene load starts.
    /// </summary>
    public static event Action<UnloadSceneStartEventArgs> OnUnloadSceneStart;
        /// <summary>
        /// RawData used by the current scene action.
        /// </summary>
        public readonly UnloadSceneQueueData RawData;

    /// <summary>
    /// Dispatched when a scene load ends.
    /// </summary>
    public static event Action<UnloadSceneEndEventArgs> OnUnloadSceneEnd;
        /// <summary>
        /// RawData used by the current scene action.
        /// </summary>
        public readonly UnloadSceneQueueData RawData;

    /// <summary>
    /// Dispatched when server receives a response from client indicating they have changed scenes. This event is called before FlexSceneCheckers are updated with this information.
    /// </summary>
    public static event Action<ClientSceneChangeEventArgs> OnClientSceneChangeStart;
        /// <summary>
        /// Scenes on the server which the client will be added to.
        /// </summary>
        public readonly List<Scene> Scenes;
        /// <summary>
        /// Connection to client which changed the scenes.
        /// </summary>
        public readonly NetworkConnection Connection;
        /// <summary>
        /// True if the scenes were loaded, false if they were unloaded. Currently this is always true as the client doesn't have to actually acknowledge unloading scenes.
        /// </summary>
        public bool Loaded;
    /// <summary>
    /// Dispatched when server receives a response from client indicating they have changed scenes. This event is called after FlexSceneCheckers are updated with this information.
    /// </summary>
    public static event Action<ClientSceneChangeEventArgs> OnClientSceneChangeEnd;
        /// <summary>
        /// Scenes on the server which the client will be added to.
        /// </summary>
        public readonly List<Scene> Scenes;
        /// <summary>
        /// Connection to client which changed the scenes.
        /// </summary>
        public readonly NetworkConnection Connection;
        /// <summary>
        /// True if the scenes were loaded, false if they were unloaded. Currently this is always true as the client doesn't have to actually acknowledge unloading scenes.
        /// </summary>
        public bool Loaded;