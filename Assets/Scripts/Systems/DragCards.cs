using UnityEngine;
using UnityEngine.InputSystem;

public class DragCards : MonoBehaviour
{
    private Card draggingCard;
    private Vector3 originalPosition;
    
    private Vector3 offset;
    private InputAction leftClickAction;
    private InputAction mousePositionAction;
    private LayerMask dragMask;
    private LayerMask dropLayermask;


    public class CardClickEventData
    {
        public Card card;
        public bool cancelled = false;

        public CardClickEventData(Card card)
        {
            this.card = card;
        }
    }
    
    public delegate void OnCardClick(CardClickEventData data);
    public static event OnCardClick CardClickEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dragMask = LayerMask.GetMask("Moveable");
        dropLayermask = LayerMask.GetMask("DropableOn");
        leftClickAction = InputSystem.actions.FindAction("LeftClick");
        mousePositionAction = InputSystem.actions.FindAction("MousePosition");
    }
    
    void Update()
    {
        // click
        if (leftClickAction.WasPressedThisFrame() && !draggingCard)
        {
            // This casts a ray into the screen and hits anything that the "dragMask" can hit - usually cards
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePositionAction.ReadValue<Vector2>()), Vector2.zero,
        float.PositiveInfinity, dragMask);
            HandleHit(hit);
        }
        // release
        else if (!leftClickAction.IsPressed() && draggingCard != null)
        {
            if (draggingCard != null) OnDrop(draggingCard);
            draggingCard = null;
        }

        if (draggingCard != null)
        {
            draggingCard.transform.position = Camera.main.ScreenToWorldPoint(mousePositionAction.ReadValue<Vector2>()) + offset;
        }
    }

    void HandleHit(RaycastHit2D hit)
    {
        if (!hit) return;
        Card card = hit.transform.GetComponent<Card>();
        if (!card) return;

        // Fires an event that can be cancelled by other scripts
        // If the event is cancelled, do not proceed
        CardClickEventData eventData = new CardClickEventData(card);
        if (CardClickEvent != null) CardClickEvent(eventData);
        if (eventData.cancelled) return;
            
        if (!card.CanMove() || !card.GetPlayer().IsHuman()) return;
        draggingCard = card;
        offset = card.transform.position - Camera.main.ScreenToWorldPoint(mousePositionAction.ReadValue<Vector2>());
        originalPosition = card.transform.position;
    }

    void OnDrop(Card card)
    {
        // Casts a ray into the screen and hits anything that the "dropLayermask" can hit
        // These are usually board objects like the playing area
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero,
            float.PositiveInfinity, dropLayermask);
        
        if (!hit)
        {
            card.TransformLerp(originalPosition);
            return;
        }
        
        Player player = card.GetPlayer();
        
        if (!player.IsTurn())
        {
            Game.Log("It's not your turn!");
            card.TransformLerp(originalPosition);
            return;
        }
        if (!player.IsActionable())
        {
            Game.Log("You can't play a card right now!");
            card.TransformLerp(originalPosition);
            return;
        }
        
        var type = hit.collider.gameObject.GetComponent<MonoBehaviour>().GetType();
        if (type == typeof(PlayingArea) && !card.HasBeenPlayed())
        {
            if (!player.GetPlayingArea().PlayCard(card))
            {
                card.TransformLerp(originalPosition);
            }
        }
        else if (type == typeof(DiscardPile))
        {
            card.Discard();
        }
        else
        {
            card.TransformLerp(originalPosition);
        }
    }
}
