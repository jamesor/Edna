using System;
using System.Collections.Generic;
using JamesOR.Edna.Interactables;
using UnityEngine;
using UnityEngine.UI;

namespace JamesOR.Edna.Player
{
    public class InventoryUI : MonoBehaviour
    {
        public Transform InventoryPanel;
        public Transform ItemsParent;
        public Button LeftButton;
        public Button RightButton;

        private Animator animator;
        private InventorySlot[] slots;

        private List<Item> m_items;
        private int m_page = 0;
        private bool m_thereIsAnotherPage = false;
        private bool m_thereIsAPreviousPage = false;

        private void Start()
        {
            animator = InventoryPanel.GetComponent<Animator>();
            slots = ItemsParent.GetComponentsInChildren<InventorySlot>();
            RightButton.interactable = m_thereIsAnotherPage;
            LeftButton.interactable = m_thereIsAPreviousPage;
        }

        private void OnEnable()
        {
            EventManager.StartListening(InventoryEvtType.Changed, OnInventoryChanged);
        }

        private void OnDisable()
        {
            EventManager.StopListening(InventoryEvtType.Changed, OnInventoryChanged);
        }

        private void UpdateUI()
        {
            if (m_items != null)
            {
                for (int i = 0, j = slots.Length * m_page; i < slots.Length; i++, j++)
                {
                    if (j < m_items.Count)
                    {
                        slots[i].AddItem(m_items[j]);
                    }
                    else
                    {
                        slots[i].ClearSlot();
                    }
                }

                m_thereIsAnotherPage = (m_items.Count > slots.Length * (m_page + 1));
                RightButton.interactable = m_thereIsAnotherPage;

                m_thereIsAPreviousPage = m_page > 0;
                LeftButton.interactable = m_thereIsAPreviousPage;
            }
        }

        public void ToggleInventory()
        {
            if (animator != null)
            {
                bool isOpen = animator.GetBool("open");
                animator.SetBool("open", !isOpen);
            }
        }

        public void OnPreviousPage()
        {
            if (m_thereIsAPreviousPage)
            {
                m_page--;
                UpdateUI();
            }
        }

        public void OnNextPage()
        {
            if (m_thereIsAnotherPage)
            {
                m_page++;
                UpdateUI();
            }
        }

        #region Event Handlers
        private void OnInventoryChanged(EventArgs e)
        {
            if (e is InventoryEvtArgs)
            {
                InventoryEvtArgs evt = (InventoryEvtArgs)e;
                m_items = evt.Items;
                UpdateUI();
            }
        }
        #endregion
    }
}
