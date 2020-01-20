using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class ChainElement : ElementBase
    {
        public int angle;
        public ChainElement(Vector3Int position) : base(position)
        {
            angle = 0;
        }
    }
}
