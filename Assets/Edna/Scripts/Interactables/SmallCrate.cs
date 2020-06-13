using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JamesOR.Edna.Interactables
{
    public class SmallCrate : Interactable
    {
        public override bool IsTakeable()
        {
            return true;
        }

        public override bool Take()
        {
            return true;
        }
    }
}
