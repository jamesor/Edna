using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JamesOR.Edna.Interactables
{
    public class SmallCrate : Interactable
    {
        public override bool Take()
        {
            return true;
        }

        public override bool Use(Item item)
        {
            base.Use(item);

            // TODO

            return false;
        }

        public override bool CanBeUsedWith(Item item)
        {
            // TODO

            return true;
        }
    }
}
