using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using Mirror;
using UnityEngine;

namespace FirstGearGames.FlexSceneManager.Demos
{

    /// <summary>
    /// Unloads specified scenes when entering or exiting this trigger.
    /// </summary>
    public class SceneUnloaderExample : MonoBehaviour
    {
        /// <summary>
        /// Scenes to unload.
        /// </summary>
        [Tooltip("Scenes to unload.")]
        [SerializeField]
        private string[] _unloadScenes;
        /// <summary>
        /// True to only unload for the connectioning causing the trigger.
        /// </summary>
        [Tooltip("True to only unload for the connectioning causing the trigger.")]
        [SerializeField]
        private bool _connectionOnly = false;
        /// <summary>
        /// True to fire when entering the trigger. False to fire when exiting the trigger.
        /// </summary>
        [Tooltip("True to fire when entering the trigger. False to fire when exiting the trigger.")]
        [SerializeField]
        private bool _onTriggerEnter = true;


        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (!_onTriggerEnter)
                return;

            UnloadScenes(other.gameObject.GetComponent<NetworkIdentity>());
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            if (_onTriggerEnter)
                return;

            UnloadScenes(other.gameObject.GetComponent<NetworkIdentity>());
        }

        /// <summary>
        /// Unload scenes.
        /// </summary>
        /// <param name="triggeringIdentity"></param>
        private void UnloadScenes(NetworkIdentity triggeringIdentity)
        {
            //NetworkIdentity isn't necessarily needed but to ensure its the player only run if netidentity is found.
            if (triggeringIdentity == null)
                return;

            AdditiveScenesData asd = new AdditiveScenesData(_unloadScenes);
            //Unload only for the triggering connection.
            if (_connectionOnly)
                FlexSceneManager.UnloadConnectionScenes(new NetworkConnection[] { triggeringIdentity.connectionToClient }, asd);
            //Unload for all players.
            else
                FlexSceneManager.UnloadNetworkedScenes(asd);
        }


    }


}