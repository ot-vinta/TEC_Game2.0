using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class Conductor : LabeledChainElement
    {
        public Conductor(Vector3Int position, int angle) : base(position, angle)
        {
            this.label.text = "Con";
            this.label.color = Color.white;
        }
        public new void FixLabel()
        {
            this.label.color = Color.white;
            if (angle % 180 == 90)
            {
                label.transform.Rotate(0, 0, 90);
            }
        }
        public Conductor(Vector3Int position, int angle, string name) : base(position, angle, name)
        {

        }
        public override string ToString()
        {
            return "Conductor";
        }
    }
}
