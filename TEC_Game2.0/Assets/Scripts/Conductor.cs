using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class Conductor : ChainElement
    {

        public Conductor(Vector3Int position, int angle) : base(position, angle)
        {
        }

        public override string ToString()
        {
            return "Conductor";
        }
    }
}
