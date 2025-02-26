using UnityEngine;

namespace Game.Player
{
    public class PlayerYSorting : MonoBehaviour
    {
        public SpriteRenderer MainRenderer;
        public SpriteRenderer Shadow;

        public int Offset = 0;

        public void FixedUpdate()
        {
            int newValue = -(int)(transform.position.y * 100) - Offset;

            MainRenderer.sortingOrder = newValue;
            if (Shadow != null)
                Shadow.sortingOrder = MainRenderer.sortingOrder - 1 - Offset;
        }
    }
}