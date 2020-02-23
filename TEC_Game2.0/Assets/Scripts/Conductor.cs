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
            this.label.text = "Проводимость";
            //this.label
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
