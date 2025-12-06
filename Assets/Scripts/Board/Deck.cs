using UnityEngine;
using System.Collections.Generic;
using Cards;
using TMPro;

public class Deck : MonoBehaviour
{
    protected Player player = null;
    /**
     * Represents the actual cards in the deck once loaded
     */
    Stack<string> cardIds = new Stack<string>();
    /**
     * The deck asset to load cards from
     */
    [SerializeField] private CardDeck staticDeck = null;
    [SerializeField] private TextMeshProUGUI cardCountText = null;
    [SerializeField] private GameObject explosionPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadDeck();
        SetPlayer();
    }
    
    public void SetStaticDeck(CardDeck deck)
    {
        staticDeck = deck;
        LoadDeck();
        UpdateText();
    }

    public void SetPlayer()
    {
        player = Player.GetPlayer(this);
    }

    private int lastCount = -1;
    // todo revisit in case this is too expensive
    void Update()
    {
        // Only update the text if the count has changed
        if (lastCount == cardIds.Count) return;
        lastCount = cardIds.Count;
        UpdateText();
    }

    // void OnMouseDown()
    // {
    //     if (!player.IsTurn() || !player.IsActionable())
    //     {
    //         Game.Log("It's not your turn!");
    //         return;
    //     }
    //     if (cardIds.Count == 0)
    //     {
    //         Game.Log("Deck is empty!");
    //         return;
    //     }
    //     
    //     DrawTopCard();
    //     Player.GetPlayer(this).TurnOver();
    // }

    public Card DrawTopCard()
    {
        if (cardIds.Count == 0 && player.GetDiscardPile().Count() == 0)
        {
            Game.Log("Deck and discard pile are empty - can't draw a card!");
            return null;
        }
        if (cardIds.Count == 0)
        {
            ShuffleDiscardPileIntoDeck(player.GetDiscardPile());
        }
        Card card = CardDatabase.InstantiateCard(cardIds.Pop(), transform);
        player.GetPlayerHand().AddCard(card);
        return card;
    }

    public void LoadDeck()
    {
        if (staticDeck == null) return;
        cardIds.Clear();
        foreach (var entry in staticDeck.cards)
        {
            for (var i = 0; i < entry.count; i++)
            {
                cardIds.Push(entry.cardName);
            }
        }
        ShuffleDeck();
    }

    // Fisher-Yates shuffle from https://stackoverflow.com/questions/2094239/swap-two-items-in-listt
    // Swap by deconstruction from https://dariuszwozniak.net/blog/swap-via-deconstruction
    private void ShuffleDeck()
    {
        var values = cardIds.ToArray();
        cardIds.Clear();
        var random = new System.Random();
        var n = values.Length;
        while (n > 1)
        {
            n--;
            var k = random.Next(n + 1);
            (values[k], values[n]) = (values[n], values[k]);
        }
        
        foreach (var value in values)
        {
            cardIds.Push(value);
        }
    }
    
    // todo add animation for this
    public void ShuffleDiscardPileIntoDeck(DiscardPile discardPile)
    {
        if (discardPile == null || discardPile.GetDiscardedCards().Count == 0)
        {
            Game.Log("Discard pile is empty!");
            return;
        }
        
        foreach (var cardId in discardPile.GetDiscardedCards())
        {
            cardIds.Push(cardId);
        }
        
        discardPile.ClearDiscardPile();
        ShuffleDeck();
        Game.Log("Discard pile shuffled into deck!");
    }

    public int Count()
    {
        return cardIds.Count;
    }

    public void Explode()
    {
        if (explosionPrefab) {
            GameObject explo = Instantiate(explosionPrefab, transform);
        }
    }

    /**
     * Deletes half the cards in the deck, rounded down, and returns the number of cards deleted
     */
    public int AnnihilateHalf()
    {
        int total = cardIds.Count / 2;
        for (int i = 0; i < total; i++)
        {
            if (cardIds.Count > 0) cardIds.Pop();
        }
        Explode();
        Game.Log("Annihilated " + total + " cards from deck!");
        return total;
    }

    private void UpdateText()
    {
        if (cardCountText) cardCountText.text = "Deck: " + cardIds.Count + " cards";
    }
}
