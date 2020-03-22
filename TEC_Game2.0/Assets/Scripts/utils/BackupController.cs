using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.utils
{
    class BackupController
    {
        private static BackupController _instance;
        private List<ElementBase> _backupElements;
        private Dictionary<ElementBase, Tile> _backupTiles;
        private Tilemap _map;

        private BackupController()
        {
            _backupElements = new List<ElementBase>();
            _backupTiles = new Dictionary<ElementBase, Tile>();
        }

        public static BackupController GetInstance()
        {
            if (_instance == null)
                _instance = new BackupController();

            return _instance;
        }

        public void Backup()
        {
            _map = GameObject.Find("Map").GetComponent<Tilemap>();

            _backupTiles.Clear();
            _backupElements = new List<ElementBase>(Scheme.elements.Values);
            foreach (var element in _backupElements)
            {
                _backupTiles.Add(element, _map.GetTile<Tile>(element.pivotPosition));
            }
        }

        public bool Restart()
        {
            _map = GameObject.Find("Map").GetComponent<Tilemap>();

            if (_backupElements.Count == 0)
            {
                return false;
            }

            foreach (var element in Scheme.elements.Values)
            {
                _map.SetTile(element.pivotPosition, new Tile());
            }

            Scheme.Clear();

            foreach (var element in _backupElements)
            {
                Scheme.AddElement(element);

                if (element is LabeledChainElement labeledChainElement)
                {
                    labeledChainElement.AddLabel(
                            labeledChainElement.labelStr, 
                            labeledChainElement.pivotPosition
                            );
                    switch (labeledChainElement)
                    {
                        case Resistor resistor:
                            resistor.FixLabel();
                            break;
                        case Conductor conductor:
                            conductor.FixLabel();
                            break;
                    }
                }

                _map.SetTile(element.pivotPosition, _backupTiles[element]);
            }

            return true;
        }

        public void ClearBackup()
        {
            _backupElements.Clear();
            _backupTiles.Clear();
        }
    }
}
