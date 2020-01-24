using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Wire : ElementBase
    {
        public Vector3Int secondPosition;

        public Wire(Vector3Int firstPosition, Vector3Int secondPosition) : base(firstPosition)
        {
            this.secondPosition = new Vector3Int(secondPosition.x, secondPosition.y, secondPosition.z);
        }
    }
}
