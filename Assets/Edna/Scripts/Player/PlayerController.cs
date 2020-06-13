using System;
using JamesOR.Edna.Interactables;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JamesOR.Edna.Player
{
    public enum ActionType { None, Look, Take, Use, Back, Skip }

    public class PlayerControllerEvtType
    {
        public static string HoldItem = "PlayerController.OnHoldItem";
        public static string ReleaseItem = "PlayerController.OnReleaseItem";
        public static string LookAtItem = "PlayerController.OnLookAtItem";
    }

    public class PlayerControllerEvtArgs : EventArgs
    {
        public Item Item { get; set; }
    }

    public class PlayerMoveEvtType
    {
        public static string MoveToPoint = "PlayerMotor.OnMoveToPoint";
    }

    public class PlayerMoveEvtArgs : EventArgs
    {
        public Vector3 point { get; set; }
    }

    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerController : MonoBehaviour
    {
        public Interactable Focused;
        public Inventory Inventory;

        private UnityEngine.Camera m_camera;
        private PlayerMotor m_motor;

        private Item m_itemBeingHeld;
        public Item ItemBeingHeld { get { return m_itemBeingHeld; } }

        private ActionType? m_deferredAction;

        private void OnEnable()
        {
            EventManager.StartListening(PlayerControllerEvtType.HoldItem, OnHoldItem);
            EventManager.StartListening(PlayerControllerEvtType.ReleaseItem, OnReleaseItem);
            EventManager.StartListening(PlayerControllerEvtType.LookAtItem, OnLookAtItem);
            EventManager.StartListening(PlayerMoveEvtType.MoveToPoint, OnMoveToPoint);
            EventManager.StartListening(PlayerMotorEvtType.ReachedDestination, OnReachDestination);
        }

        private void OnDisable()
        {
            EventManager.StopListening(PlayerControllerEvtType.HoldItem, OnHoldItem);
            EventManager.StopListening(PlayerControllerEvtType.ReleaseItem, OnReleaseItem);
            EventManager.StopListening(PlayerControllerEvtType.LookAtItem, OnLookAtItem);
            EventManager.StopListening(PlayerMoveEvtType.MoveToPoint, OnMoveToPoint);
            EventManager.StopListening(PlayerMotorEvtType.ReachedDestination, OnReachDestination);
        }

        private void Start()
        {
            m_camera = UnityEngine.Camera.main;
            m_motor = GetComponent<PlayerMotor>();
            Inventory = new Inventory();
        }

        private void Update()
        {
            // Don't fall through UI Buttons
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // Left Button
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit))
                {
                    // Debug.Log("We hit " + hit.collider.name + " at " + hit.point);
                    Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
                    if (interactable != null && interactable.Item != null)
                    {
                        if (m_itemBeingHeld == null)
                        {
                            if (interactable.IsTakeable())
                            {
                                m_deferredAction = ActionType.Take;
                            }
                            else if (interactable.IsUseable())
                            {
                                m_deferredAction = ActionType.Use;
                            }
                        }
                        else
                        {
                            bool isItemsUseableHere = interactable.CanBeUsedWith(m_itemBeingHeld);
                            if (isItemsUseableHere)
                            {
                                m_deferredAction = ActionType.Use;
                            }
                            else
                            {
                                return;
                            }
                        }

                        SetFocus(interactable);
                    }
                    else
                    {
                        m_motor.MoveToPoint(hit.point);
                        RemoveFocus();
                    }
                }
            }

            // Right Button
            if (Input.GetMouseButtonDown(1))
            {
                if (m_itemBeingHeld != null)
                {
                    ReleaseItem();
                    return;
                }

                Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Debug.Log("We hit " + hit.collider.name + " at " + hit.point);
                    Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
                    if (interactable == null)
                    {
                        m_motor.MoveToPoint(hit.point);
                        RemoveFocus();
                    }
                    else
                    {
                        LookAtItem(interactable.Item);
                    }
                }
            }
        }

        private void HandleDefferedAction()
        {
            if (Focused != null)
            {
                switch (m_deferredAction)
                {
                    case ActionType.Take:
                        if (Focused.Take() && Inventory.Add(Focused.Item))
                        {
                            Destroy(Focused.gameObject);
                        }
                        break;
                    case ActionType.Use:
                        Focused.Use(m_itemBeingHeld);
                        break;
                    default:
                        break;
                }
            }

            m_deferredAction = null;
        }

        private void SetFocus(Interactable newFocus)
        {
            if (newFocus == Focused)
            {
                HandleDefferedAction();
            }
            else
            {
                Focused = newFocus;
                m_motor.FollowTarget(newFocus);
            }
        }

        private void RemoveFocus()
        {
            Focused = null;
            m_motor.StopFollowingTarget();
        }

        private void HoldItem(Item item)
        {
            if (item != null)
            {
                m_itemBeingHeld = item;
            }
        }

        private void ReleaseItem()
        {
            m_itemBeingHeld = null;
        }

        private void LookAtItem(Item item)
        {
            if (item != null)
            {
                Debug.Log(item.Description);
            }
        }

        #region Event Handlers
        private void OnReachDestination(EventArgs e)
        {
            HandleDefferedAction();
        }

        private void OnHoldItem(EventArgs e)
        {
            ReleaseItem();

            if (e is PlayerControllerEvtArgs)
            {
                PlayerControllerEvtArgs evt = (PlayerControllerEvtArgs)e;
                HoldItem(evt.Item);
            }
        }

        private void OnReleaseItem(EventArgs e)
        {
            ReleaseItem();
        }

        private void OnLookAtItem(EventArgs e)
        {
            if (e is PlayerControllerEvtArgs)
            {
                PlayerControllerEvtArgs evt = (PlayerControllerEvtArgs)e;
                LookAtItem(evt.Item);
            }
        }

        private void OnMoveToPoint(EventArgs e)
        {
            if (e is PlayerMoveEvtArgs)
            {
                PlayerMoveEvtArgs evt = (PlayerMoveEvtArgs)e;
                m_motor.MoveToPoint(evt.point);
            }
        }
        #endregion
    }
}
