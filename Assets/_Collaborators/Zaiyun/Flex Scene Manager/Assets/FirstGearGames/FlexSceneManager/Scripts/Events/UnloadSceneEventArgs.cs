using FirstGearGames.FlexSceneManager.LoadUnloadDatas;

namespace FirstGearGames.FlexSceneManager.Events
{


    public struct UnloadSceneStartEventArgs
    {
        /// <summary>
        /// RawData used by the current scene action.
        /// </summary>
        public readonly UnloadSceneQueueData RawData;

        public UnloadSceneStartEventArgs(UnloadSceneQueueData sqd)
        {
            RawData = sqd;
        }
    }

    public struct UnloadSceneEndEventArgs
    {
        /// <summary>
        /// RawData used by the current scene action.
        /// </summary>
        public readonly UnloadSceneQueueData RawData;

        public UnloadSceneEndEventArgs(UnloadSceneQueueData sqd)
        {
            RawData = sqd;
        }
    }


}