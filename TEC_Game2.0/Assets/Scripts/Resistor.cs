using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Resistor : ChainElement
    {
        
        public Resistor(Vector3Int position, int angle) : base(position, angle)
        {
        }

        public override string ToString()
        {
            return "Resistor";
        }
    }
}
