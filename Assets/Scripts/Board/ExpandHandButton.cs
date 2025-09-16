using UnityEngine;

namespace Board
{
    /**
     * A button that expands and collapses the player's hand if there are too many cards.
     */
    public class ExpandHandButton : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer; 
        private Hand hand;
        [SerializeField] private Sprite expandSprite;
        [SerializeField] private Sprite collapseSprite;
        
        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        void OnMouseDown()
        {
            if (!hand) return;
            if (!hand.IsExpanded())
            {
                hand.Expand();
            }
            else
            {
                hand.Contract();
            }
        }

        public void UpArrowSprite()
        {
            spriteRenderer.sprite = expandSprite;
        }
        
        public void DownArrowSprite()
        {
            spriteRenderer.sprite = collapseSprite;
        }
        
        public void SetHand(Hand h)
        {
            hand = h;
        }
    }
}