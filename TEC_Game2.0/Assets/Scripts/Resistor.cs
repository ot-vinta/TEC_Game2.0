using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Resistor : LabeledChainElement
    {
        public Resistor(Vector3Int position, int angle) : base(position, angle)
        {
            this.label.text = "Сопротивление";
        }
        public Resistor(Vector3Int position, int angle, string name) : base(position, angle, name)
        {

        }
    }
}
