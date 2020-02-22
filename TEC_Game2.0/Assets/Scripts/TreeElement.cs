using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class TreeElement
    {
        public ElementBase element;
        public Vector2Int connectPosition;

        public TreeElement(ElementBase element, Vector2Int position)
        {
            this.element = element;
            connectPosition = position;
        }
    }
}
