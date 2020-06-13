using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JamesOR.Edna
{
    public class CameraManager : Singleton<CameraManager>
    {
        public Transform StartPosition;

        protected override void OnAwake()
        {
            if (StartPosition != null)
            {
                MoveCameraTo(StartPosition);
            }
        }

        public static void MoveCameraTo(Transform transform)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;

            UnityEngine.Camera.main.transform.position = pos;
            UnityEngine.Camera.main.transform.rotation = rot;
        }
    }
}
