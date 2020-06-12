using JamesOR.Edna.Interactables;
using JamesOR.Edna.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JamesOR.Edna.Player
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public Image Icon;
        public Text Title;

        Item m_item;

        public void AddItem(Item newItem)
        {
            m_item = newItem;

            Icon.sprite = newItem.Icon;
            Icon.enabled = true;
        }

        public void ClearSlot()
        {
            m_item = null;

            Icon.sprite = null;
            Icon.enabled = false;
            Title.text = "";
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_item != null)
            {
                Title.text = m_item.Name;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Title.text = "";
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (m_item != null)
            {
                // Left Button
                if (Input.GetMouseButtonDown(0))
                {
                    PlayerControllerEvtArgs args = new PlayerControllerEvtArgs();
                    args.Item = m_item;
                    EventManager.TriggerEvent(PlayerControllerEvtType.HoldItem, args);
                }

                // Right Button
                if (Input.GetMouseButtonDown(1))
                {
                    PlayerControllerEvtArgs args = new PlayerControllerEvtArgs();
                    args.Item = m_item;
                    EventManager.TriggerEvent(PlayerControllerEvtType.LookAtItem, args);
                }
            }
        }
    }
}
