using UnityEngine;
using UnityEngine.InputSystem;

namespace Board
{
    public class HoverCards : MonoBehaviour
    {
        private Transform hoveredTransform;
        private LayerMask hitMask;
        private InputAction mousePositionAction;
        private InputAction leftClickAction;


        void Start()
        {
            hitMask = LayerMask.GetMask("Moveable");
            mousePositionAction = InputSystem.actions.FindAction("MousePosition");
            leftClickAction = InputSystem.actions.FindAction("LeftClick");
        }

        void Update()
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePositionAction.ReadValue<Vector2>()), Vector2.zero,
                float.PositiveInfinity, hitMask);
            if (!hit && !hoveredTransform) return;

            if (!hit)
            {
                ResetHovered();
                return;
            }
            
            if (hit.transform != hoveredTransform)
            {
                if (hoveredTransform)
                {
                    ResetHovered();
                }

                hoveredTransform = hit.transform;
                Card card = hoveredTransform.GetComponent<Card>();
                SpriteRenderer newSprite = card.GetSpriteRenderer();
                if (card && !leftClickAction.IsPressed())
                {
                    card.Wiggle();
                    card.DisplayDescription(true);
                }
                newSprite.sortingOrder += 10;
            }
        }

        private void ResetHovered()
        {
            //Stops the card description from being displayed
            Card card = hoveredTransform.GetComponent<Card>();
            if (card)
            {
                card.DisplayDescription(false);
            }

            SpriteRenderer sprite = card.GetSpriteRenderer();
            // todo this is unsafe if something else changed the sorting order
            // this is visible after using Fishing - the target card is placed in a negative sorting order
            sprite.sortingOrder -= 10;
            if (card)
            {
                card.DisplayDescription(false);
            }

            hoveredTransform = null;
        }

        
    }
}