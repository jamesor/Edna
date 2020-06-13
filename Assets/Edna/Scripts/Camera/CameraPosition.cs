using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JamesOR.Edna.Camera
{
    public class CameraPosition : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, .2f);
        }
    }
}

