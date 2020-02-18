using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public abstract class ElementBase
    {
        public Vector3Int pivotPosition;
        public int id;
        public int angle;

        protected ElementBase(Vector3Int position, int angle)
        {
            this.pivotPosition = new Vector3Int(position.x, position.y, position.z);
            this.angle = angle;
            id = 0;
        }

        public void SetId(int id)
        {
            this.id = id;
        }

        public int GetId()
        {
            return id;
        }

        public override string ToString()
        {
            return "ElementBase";
        }
    }
}
