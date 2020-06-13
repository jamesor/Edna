using System;
using JamesOR.Edna.Interactables;
using UnityEngine;
using UnityEngine.AI;

namespace JamesOR.Edna.Player
{
    public class PlayerMotorEvtType
    {
        public static string ReachedDestination = "PlayerMotor.OnReachedDestination";
    }

    public class PlayerMotorEvtArgs : EventArgs
    {
        public Item Item { get; set; }
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerMotor : MonoBehaviour
    {
        private Transform m_target;
        private NavMeshAgent m_agent;
        private float m_rotationSpeed = 5f;
        private bool m_reachedDestination = true;

        private void Start()
        {
            m_agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (m_target != null)
            {
                m_agent.SetDestination(m_target.position);
                FaceTarget();
            }

            if (!m_reachedDestination && !m_agent.pathPending)
            {
                if (m_agent.remainingDistance <= 0.1f)
                {
                    if (!m_agent.hasPath || m_agent.velocity.sqrMagnitude == 0f)
                    {
                        m_target = null;
                        m_reachedDestination = true;
                        EventManager.TriggerEvent(PlayerMotorEvtType.ReachedDestination, null);
                    }
                }
            }
        }

        private void FaceTarget()
        {
            Vector3 direction = (m_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * m_rotationSpeed);
        }

        public void MoveToPoint(Vector3 point)
        {
            if (point != null)
            {
                m_reachedDestination = false;
                m_agent.SetDestination(point);
            }
        }

        public void FollowTarget(Interactable newTarget)
        {
            if (newTarget != null)
            {
                m_reachedDestination = false;
                //agent.stoppingDistance = newTarget.radius * .2f;
                m_agent.updateRotation = false;

                m_target = newTarget.InteractionTransform;
            }
        }

        public void StopFollowingTarget()
        {
            //agent.stoppingDistance = 0f;
            m_agent.updateRotation = true;

            m_target = null;
        }
    }
}
