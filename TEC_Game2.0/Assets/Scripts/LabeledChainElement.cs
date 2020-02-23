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
    public class LabeledChainElement : ChainElement
    {
        public Text label;
        public LabeledChainElement(Vector3Int position, int angle) : base(position, angle)
        {
            this.AddLabel("Элемент цепи", position);
        }

        public void AddLabel(string label, Vector3Int position)
        {
            GameObject labels = GameObject.Find("Labels");
            Text template = labels.GetComponentInChildren<Text>();
            Tilemap tilemap = GameObject.FindObjectOfType<Tilemap>();
            Vector3 worldPosition = new Vector3(tilemap.CellToWorld(position).x + (float) 0.3, tilemap.CellToWorld(position).y + (float) 0.1, tilemap.CellToWorld(position).z);

            this.label = UnityEngine.Object.Instantiate(template, worldPosition, Quaternion.identity, labels.transform);

            this.label.text = label;
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
