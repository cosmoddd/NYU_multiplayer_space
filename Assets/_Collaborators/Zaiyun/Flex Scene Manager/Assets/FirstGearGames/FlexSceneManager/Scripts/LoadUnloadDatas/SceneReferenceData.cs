using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager.LoadUnloadDatas
{

    /// <summary>
    /// Contains information to look up a scene.
    /// </summary>
    public class SceneReferenceData
    {
        /// <summary>
        /// Handle of the scene. If value is 0, then handle is not used.
        /// </summary>
        public int Handle;
        /// <summary>
        /// Name of the scene.
        /// </summary>
        public string Name;

        public SceneReferenceData() { }
        public SceneReferenceData(Scene scene)
        {
            Handle = scene.handle;
            Name = scene.name;
        }
    }


}