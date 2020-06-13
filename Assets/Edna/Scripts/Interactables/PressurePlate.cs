using JamesOR.Edna.Player;
using UnityEngine;

namespace JamesOR.Edna.Interactables
{
    public class PressurePlate : Interactable
    {
        public SecretBookcase SecretBookcase;

        private Vector3 m_placementOffset = new Vector3(.125f, 0f, .125f);

        public override bool IsUseable()
        {
            return true;
        }

        public override bool Use(Item item)
        {
            if (SecretBookcase != null && CanBeUsedWith(item))
            {
                PlayerControllerEvtArgs pcArgs = new PlayerControllerEvtArgs();
                EventManager.TriggerEvent(PlayerControllerEvtType.ReleaseItem, pcArgs);

                InventoryEvtArgs invArgs = new InventoryEvtArgs();
                invArgs.Items.Add(item);
                EventManager.TriggerEvent(InventoryEvtType.RemoveItems, invArgs);

                if (item.Prefab != null)
                {
                    GameObject go = Instantiate(item.Prefab, transform.position + m_placementOffset, Quaternion.identity);
                    go.transform.parent = this.transform;
                }

                return SecretBookcase.Open();
            }

            return false;
        }

        public override bool CanBeUsedWith(Item item)
        {
            return (item != null && item.Key == ItemKey.SmallCrate);
        }
    }
}
