using FirstGearGames.FlexSceneManager.LoadUnloadDatas;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace FirstGearGames.FlexSceneManager.Demos
{

    /// <summary>
    /// Loads a single scene, additive scenes, or both when a client
    /// enters or exits this trigger.
    /// </summary>
    public class SceneLoaderExample : MonoBehaviour
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
        /// True to move all connection identities (local players).
        /// </summary>
        [Tooltip("True to move all connection identities (local players).")]
        [SerializeField]
        private bool _moveAllIdentities = false;
        /// <summary>
        /// Additive scenes to load. Leave empty to not load additive scenes.
        /// </summary>
        [Tooltip("Additive scenes to load. Leave empty to not load additive scenes.")]
        [SerializeField]
        private string[] _additiveScenes;
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

            LoadScene(other.GetComponent<NetworkIdentity>());
        }

        [ServerCallback]
        private void OnTriggerExit(Collider other)
        {
            if (_onTriggerEnter)
                return;

            LoadScene(other.GetComponent<NetworkIdentity>());
        }

        private void LoadScene(NetworkIdentity triggeringIdentity)
        {
            //NetworkIdentity isn't necessarily needed but to ensure its the player only run if found.
            if (triggeringIdentity == null)
                return;

            SingleSceneData ssd = null;
            //If to load a single scene.
            if (_singleScene != string.Empty)
            {
                List<NetworkIdentity> movedIdents = new List<NetworkIdentity>();
                if (_moveAllIdentities)
                {
                    foreach (var item in NetworkServer.connections)
                        movedIdents.Add(item.Value.identity);
                }
                else if (_moveIdentity)
                {
                    movedIdents.Add(triggeringIdentity);
                }

                ssd = new SingleSceneData(_singleScene, movedIdents.ToArray());
            }

            //Additive.
            AdditiveScenesData asd = null;
            if (_additiveScenes != null && _additiveScenes.Length > 0)
                asd = new AdditiveScenesData(_additiveScenes);

            //Load for connection only.
            if (_connectionOnly)
                FlexSceneManager.LoadConnectionScenes(new NetworkConnection[] { triggeringIdentity.connectionToClient }, ssd, asd, true);
            //Load for all clients.
            else
                FlexSceneManager.LoadNetworkedScenes(ssd, asd);


        }


    }



}