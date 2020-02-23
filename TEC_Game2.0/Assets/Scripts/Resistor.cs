using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{

    [Serializable]
    public class Resistor : LabeledChainElement
    {
        public Resistor(Vector3Int position, int angle) : base(position, angle)
        {
            this.label.text = "R";
        }
        public new void FixLabel()
        {
            this.label.color = Color.black;
            if (angle % 180 == 90)
            {
                label.transform.Rotate(0, 0, 90);
            }
        }
        public Resistor(Vector3Int position, int angle, string name) : base(position, angle, name)
        {

        }
        public override string ToString()
        {
            return "Resistor";
        }
        
    }
}
