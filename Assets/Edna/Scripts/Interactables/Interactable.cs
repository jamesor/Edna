using UnityEngine;

namespace JamesOR.Edna.Interactables
{
    public class Interactable : MonoBehaviour
    {
        public float Radius = 3f;
        public Transform InteractionTransform;
        public Item Item;

        private void Start()
        {
            if (Item == null)
            {
                Debug.LogWarning("Iteractable " + gameObject.name + " requires an item.");
            }
        }

        public virtual bool Take()
        {
            return false;
        }

        public virtual bool Use(Item item)
        {
            if (!item.IsReusable)
            {
                // TODO: Remove item from inventory
            }

            Debug.Log("Use " + item.name + " on " + Item.name);

            return false;
        }

        public virtual bool CanBeUsedWith(Item item)
        {
            // TODO

            return true;
        }

        private void OnDrawGizmosSelected()
        {
            if (InteractionTransform == null)
            {
                InteractionTransform = transform;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(InteractionTransform.position, Radius);
        }
    }
}
