using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class LabeledChainElement : ChainElement
    {
        public string name;
        public LabeledChainElement(Vector3Int position, int angle) : base(position, angle)
        {

        }
        public LabeledChainElement(Vector3Int position, int angle, string name) : base(position, angle)
        {
            this.name = name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }
}
