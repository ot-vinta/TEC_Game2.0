﻿using System;
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
            this.SetName("Con");
            this.label.color = Color.white;
        }
        public new void FixLabel()
        {
            base.FixLabel();
            this.label.color = Color.white;
        }
        public new void SetName(string name)
        {
            base.SetName(name);
            this.FixLabel();
        }
        public new void AddLabel(string label, Vector3Int position)
        {
            base.AddLabel(label, position);
            this.FixLabel();
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
