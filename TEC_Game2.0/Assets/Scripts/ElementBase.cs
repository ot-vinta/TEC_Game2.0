using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class ElementBase
    {
        public Vector3Int pivotPosition;
        private int id;

        protected ElementBase(Vector3Int position)
        {
            this.pivotPosition = new Vector3Int(position.x, position.y, position.z);
            id = 0;
        }

        public void SetId(int id)
        {
            this.id = id;
        }
    }
}
