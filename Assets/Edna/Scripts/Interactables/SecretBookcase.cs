using System.Collections;
using System.Collections.Generic;
using JamesOR.Edna.Player;
using UnityEngine;

namespace JamesOR.Edna.Interactables
{
    [RequireComponent(typeof(Animation))]
    public class SecretBookcase : Interactable
    {
        private Animation m_animation;

        private void Start()
        {
            m_animation = GetComponent<Animation>();
        }

        public bool Open()
        {
            PlayerMoveEvtArgs args = new PlayerMoveEvtArgs();
            args.point = new Vector3(2.8f, 0.0f, -4.0f);
            EventManager.TriggerEvent(PlayerMoveEvtType.MoveToPoint, args);

            m_animation.Play();
            
            return true;
        }
    }
}
