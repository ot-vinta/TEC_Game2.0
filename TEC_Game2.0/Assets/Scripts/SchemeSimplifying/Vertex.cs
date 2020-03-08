using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.SchemeSimplifying
{
    class Vertex
    {
        public int id;
        public List<Vector2Int> positions;

        public Vertex(int id)
        {
            this.id = id;
            positions = new List<Vector2Int>();
        }

        public void AddPosition(Vector2Int pos)
        {
            positions.Add(pos);
        }
    }
}
