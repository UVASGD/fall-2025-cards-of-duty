using System.Collections;
using System.Collections.Generic;
using Board;
using UnityEngine;

/**
 * Represents a player's hand of cards.
 * Cards in the hand are children of this GameObject.
 */
public class Hand : MonoBehaviour
{
    
    /**
     * Left alignment means cards fill in from left-to-right.
     */
    public enum Alignment
    {
        Left,
        Right
    }

    // Maximum dimensions of the hand
    [SerializeField] private float maxLeft = 5;
    [SerializeField] private float maxRight = 5;
    [SerializeField] public Alignment alignment = Alignment.Left;
    [SerializeField] private ExpandHandButton expandHandButton;
    /**
     * The gap between horizontal cards in the hand, in Unity world units
     */
    private float gap = 1.5f;
    /**
     * The gap between vertical rows of cards in the hand, in Unity world units
     */
    private float verticalOffset = 2.5f;
    private int maxPerRow = 6;
    
    /**
     * Holds the original position of the hand if it changes
     */
    private Vector3 originalPosition;
    private bool expanded;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
        if (expandHandButton) expandHandButton.SetHand(this);
        if (expandHandButton) expandHandButton.gameObject.SetActive(false);
        // todo temp
        if (transform.position.y > 0) verticalOffset = -verticalOffset;
    }

    public int CountCards()
    {
        return transform.childCount;
    }

    /**
     * Returns the card at the specified index, or null if out of range
     * Specifically looks at the children of this GameObject
     */
    public Card GetCard(int index)
    {
        if (index >= transform.childCount) return null;
        Card card = transform.GetChild(index).GetComponent<Card>();
        return card;
    }

    // todo break up and move to CPUPlayer class, since this is only used by CPU
    public void PlayCard(int index)
    {
        Card card = GetCard(index);
        Player.GetPlayer(this).GetPlayingArea().PlayCard(card);
    }

    /**
     * Adds a card to the hand and updates the positions of all cards in the hand.
     * Additionally shows/hides the expand hand button if card count exceeds a threshold
     */
    public void AddCard(Card card)
    {
        if (!card) return;
        var relativePosition = CalculateCardLocation(transform.childCount);
        card.GetSpriteRenderer().sortingOrder = transform.childCount;
        card.transform.SetParent(transform, true);
        card.TransformLerp(relativePosition);
        if (card.GetPlayer().IsHuman()) card.Show();
        
        // Count cards
        if (expandHandButton)
        {
            if (transform.childCount > maxPerRow)
            {
                expandHandButton.gameObject.SetActive(true);
            }
            else
            {
                expandHandButton.gameObject.SetActive(false);
                Contract();
            }
        }
    }
    /**
     * Starts a coroutine to update the positions of all cards in the hand.
     */
    public void UpdateCardLocations()
    {
        StartCoroutine(UpdateCardLocationsCoroutine());
    }
    
    private IEnumerator UpdateCardLocationsCoroutine()
    {
        yield return 0;
        for (var i = 0; i < transform.childCount; i++)
        {
            var card = transform.GetChild(i).GetComponent<Card>();
            if (!card) continue;
            var newPosition = CalculateCardLocation(i);
            card.TransformLerp(newPosition);
            card.GetSpriteRenderer().sortingOrder = i;
        }
    }

    
    private Vector3 CalculateCardLocation(int index)
    {
        // Determine row and column
        int horizontal = index % maxPerRow;
        int vertical = index / maxPerRow;
        
        // Slight indent for odd rows
        float indent = gap / 3.0f;
        Vector3 point = Vector3.zero;
        switch (alignment)
        {
            case Alignment.Left:
                point = transform.TransformPoint(new Vector3(-maxLeft + horizontal * gap, 0, 0));
                point.y -= vertical * verticalOffset;
                if (vertical % 2 == 1) point.x -= indent;
                break;
            case Alignment.Right:
                point = transform.TransformPoint(new Vector3(maxRight - horizontal * gap, 0, 0));
                point.y -= vertical * verticalOffset;
                if (vertical % 2 == 1) point.x += indent;
                break;
        }

        return point;
    }

    /**
     * Merges cards of the same type into a single "card stack". Not used at the moment.
     */
    public void MergeCards()
    {
        Dictionary<string, List<Card>> cardsTypeSorted = new Dictionary<string, List<Card>>();
        foreach (Transform t in transform)
        {
            var card = t.GetComponent<Card>();
            if (!card.IsPlayable()) continue;
            var id = card.GetId();
            cardsTypeSorted.TryAdd(id, new List<Card>());
            cardsTypeSorted[id].Add(card);
        }

        // Merge cards with frequency > 1
        foreach (var pair in cardsTypeSorted)
        {
            var cardList = pair.Value;
            if (cardList.Count <= 1) continue;
            int totalStacks = 0;
            for (int i = 0; i < cardList.Count; i++)
            {
                var card = cardList[i];
                totalStacks += card.GetStacks();
                if (i != 0) Destroy(card.gameObject);
            }
            cardList[0].SetStacks(totalStacks);
        }
    }

    /**
     * Expands the hand to show all cards.
     * Currently is scuffed.
     */
    public void Expand()
    {
        int rows = transform.childCount / maxPerRow + 1;
        Vector3 up = new Vector3(0, (float) (rows * 3.5 - 4.5), 0);
        transform.position = originalPosition + up;
        UpdateCardLocations();
        expanded = true;
        expandHandButton.DownArrowSprite();
    }
    
    /**
     * Contracts the hand back to its original position.
     */
    public void Contract()
    {
        transform.position = originalPosition;
        UpdateCardLocations();
        expanded = false;
        expandHandButton.UpArrowSprite();
    }
    
    public bool IsExpanded()
    {
        return expanded;
    }
}
