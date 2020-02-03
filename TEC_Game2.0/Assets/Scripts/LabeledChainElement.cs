using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class LabeledChainElement : ChainElement
    {
        public Text label;
        public LabeledChainElement(Vector3Int position, int angle) : base(position, angle)
        {
            //this.label = GameObject.Find("Map").AddComponent<Text>();
            //
            //this.label.transform.position = new Vector3Int(position.x, position.y + 1, position.z);

            Text template = GameObject.Find("Labels").GetComponentInChildren<Text>();
            this.label = UnityEngine.Object.Instantiate(template, new Vector3Int(position.x, position.y + 1, position.z), new Quaternion(0,0,0,0));
            this.label.text = "Элемент цепи";
            this.label.enabled = true;

        }
        public LabeledChainElement(Vector3Int position, int angle, string name) : base(position, angle)
        {
            this.label.text = name;
        }

        public void SetName(string name)
        {
            this.label.text = name;
        }
    }
}
