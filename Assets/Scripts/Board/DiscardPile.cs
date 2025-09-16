using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DiscardPile : MonoBehaviour
{
    SpriteRenderer spriteRenderer = null;
    /**
     * Sprite for empty discard pile
     */
    Sprite emptySprite = null;
    private Player player = null;
    Stack<string> discardedCards = new Stack<string>();
    [SerializeField] private TextMeshProUGUI cardCountText = null;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = Player.GetPlayer(this);
        emptySprite = spriteRenderer.sprite;
    }

    private int lastCount = -1;
    // todo revisit in case this is too expensive
    void Update()
    {
        // Only update the text if the count has changed
        if (lastCount == discardedCards.Count) return;
        lastCount = discardedCards.Count;
        UpdateText();
    }

    // void OnMouseDown()
    // {
    //     if (!player.IsTurn())
    //     {
    //         Debug.Log("It's not your turn!");
    //         return;
    //     }
    //     
    //     Deck deck = player.GetDeck();
    //     if (deck == null)
    //     {
    //         Debug.Log("There is no deck??");
    //     }
    //
    //     if (deck.Count() == 0)
    //     {
    //         deck.ShuffleDiscardPileIntoDeck(this);
    //     }
    //     else
    //     {
    //         Debug.Log("You can only shuffle your discard pile back into your deck when it is empty!");
    //     }
    // }

    public void PushCard(Card card)
    {
        spriteRenderer.sprite = card.GetSpriteRenderer().sprite;
        discardedCards.Push(card.GetId());
    }
    
    public Stack<string> GetDiscardedCards()
    {
        return discardedCards;
    }
    
    public void ClearDiscardPile()
    {
        discardedCards.Clear();
        spriteRenderer.sprite = emptySprite;
    }

    public int Count()
    {
        return discardedCards.Count;
    }
    
    private void UpdateText()
    {
        if (cardCountText) cardCountText.text = "Discard Pile: " + discardedCards.Count + " cards";
    }
}
