using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JamesOR.Edna.Camera
{
    public class CameraTrigger : MonoBehaviour
    {
        public Transform EnterCamera;
        public Transform ExitCamera;

        private void OnTriggerEnter(Collider collider)
        {
            if (EnterCamera != null && collider.tag == "Player")
            {
                CameraManager.MoveCameraTo(EnterCamera);
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (ExitCamera != null && collider.tag == "Player")
            {
                CameraManager.MoveCameraTo(ExitCamera);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = (ExitCamera == null) ? Color.blue : Color.red;
            Gizmos.DrawWireCube(GetComponent<BoxCollider>().bounds.center, GetComponent<BoxCollider>().bounds.size);
        }
    }
}
