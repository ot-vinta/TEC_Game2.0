using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
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

            GameObject labels = GameObject.Find("Labels");
            Text template = labels.GetComponentInChildren<Text>();
            //this.label = UnityEngine.Object.Instantiate(template, new Vector3Int(position.x, position.y + 1, position.z), new Quaternion(0,0,0,0));


            Tilemap tilemap = GameObject.FindObjectOfType<Tilemap>();
            Vector3 worldPosition = tilemap.CellToWorld(position);
            Vector3Int worldPositionInt = new Vector3Int((int)worldPosition.x, (int)worldPosition.y, (int)worldPosition.z);

            this.label = UnityEngine.Object.Instantiate(template, worldPositionInt, Quaternion.identity, labels.transform);
            //this.label = UnityEngine.Object.Instantiate(template, position, Quaternion.identity, tilemap.transform);

            //this.labe

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
