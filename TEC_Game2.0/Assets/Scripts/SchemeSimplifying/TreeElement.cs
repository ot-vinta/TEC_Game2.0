using UnityEngine;

namespace Assets.Scripts.SchemeSimplifying
{
    class TreeElement
    {
        public Vector2Int Position;
        public TreeElement Root;
        public bool IsDeleted;
        public ElementBase ElementToRoot;

        public TreeElement(Vector2Int position, TreeElement root, ElementBase element)
        {
            this.Position = position;
            this.Root = root;
            IsDeleted = false;
            ElementToRoot = element;
        }

        public void Delete()
        {
            IsDeleted = false;
        }
    }
}
