using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstGearGames.FlexSceneManager.Demos
{


    public class PlayerController : NetworkBehaviour
    {
        public bool Unload = false;
        public bool DoIt = true;
        public bool MoveIdentity = true;
        [SerializeField]
        private GameObject _randomObjectPrefab;
        [SerializeField]
        private GameObject _camera;
        [SerializeField]
        private float _moveRate = 4f;


        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _camera.SetActive(true);
        }

        private void Update()
        {
            if (!base.hasAuthority)
                return;

            float gravity = -10f * Time.deltaTime;
            //If ray hits floor then cancel gravity.
            Ray ray = new Ray(transform.position + new Vector3(0f, 0.05f, 0f), -Vector3.up);
            if (Physics.Raycast(ray, 0.1f + -gravity))
                gravity = 0f;

            /* Moving. */
            Vector3 direction = new Vector3(
                0f,
                gravity,
                Input.GetAxisRaw("Vertical") * _moveRate * Time.deltaTime);

            transform.position += transform.TransformDirection(direction);
            transform.Rotate(new Vector3(0f, Input.GetAxisRaw("Horizontal"), 0f));

            if (Input.GetKeyDown(KeyCode.R))
                CmdSpawnRandomObject();
        }

        [Command]
        private void CmdSpawnRandomObject()
        {
            //Initiate object normally.
            GameObject go = Instantiate(_randomObjectPrefab, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
            
            //If streaming blue scene is loaded, put it into that scene.
            Scene s = SceneManager.GetSceneByName("StreamingBlue");
            if (!string.IsNullOrEmpty(s.name))
                SceneManager.MoveGameObjectToScene(go, s);

            //Network spawn.
            NetworkServer.Spawn(go);
        }
    }


}