using CardBehaviors;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Define which player state this card belongs to
    protected Player player = null;
    /**
     * Whether you can drag this card (disabled when card is automatically moving)
     */
    private bool moveable = true;
    /**
     * Whether this card may be played
     */
    private bool playable = true;
    /**
     * Whether this card has been played this turn
     */
    private bool hasBeenPlayed = false;
    /**
     * Whether this card is highlighted (selected)
     */
    private bool highlighted = false;

    /**
     * Number of cards in this stack
     */
    private int stacks = 1;
    
    // todo rename this to "movelerping"
    private bool lerping = false;
    private float lerpTime = 0;
    private const float lerpDuration = 0.25f;
    private Vector3 startTransform = Vector3.zero;
    private Vector3 targetTransform = Vector3.zero;

    private bool scaleLerping = false;
    private float scaleLerpTime = 0;
    private const float scaleLerpDuration = 0.25f;
    private Vector3 startScale = Vector3.zero;
    private Vector3 targetScale = Vector3.zero;
    
    [SerializeField] private Sprite faceDownSprite;
    /**
     * This is set to the current sprite in the SpriteRenderer on Awake()
     */
    private Sprite faceUpSprite;
    public SpriteRenderer spriteRenderer;
    [SerializeField] public GameObject sprite;
    [SerializeField] public GameObject playText;
    [SerializeField] public GameObject stackText;
    [SerializeField] public GameObject xText;
    [SerializeField] public GameObject cardDescription;
    public TextMeshProUGUI descriptionText;
    
    /**
     * Destroy this card upon completing an automatic movement (e.g. moving to the discard pile)
     */
    private bool destroyWhenLerpComplete = false;
    
    private string id;
    private string cardName;
    [SerializeField] private bool starCard = false;

    // Behavior scripts
    [SerializeField] public BoardBehavior boardBehavior = null;
    [SerializeField] public PlayBehavior playBehavior = null;
    [SerializeField] public VisualBehavior visualBehavior = null;
    [SerializeField] public DiscardBehavior discardBehavior = null;
    private Animator animator = null;
    
    [SerializeField] private GameObject starCardEffectPrefab = null;
    
    // todo make this cancellable
    public delegate void PlayAction(Card card);
    public static event PlayAction CardPlayEvent;

    public void Play()
    {
        if (starCard) PlayStarCardEffect();
        StartCoroutine(playBehavior.Play());
        if (CardPlayEvent != null) CardPlayEvent(this);
        hasBeenPlayed = true;
    }
    
    public void Discard()
    {
        discardBehavior.Discard();
    }

    void Awake()
    {
        player = Player.GetPlayer(this);
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        faceUpSprite = spriteRenderer.sprite;
        animator = GetComponent<Animator>();
        if (playText) playText.SetActive(false);
        if (stackText) stackText.SetActive(false);
        if (xText) xText.SetActive(false);
        if (cardDescription) descriptionText = cardDescription.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Show()
    {
        spriteRenderer.sprite = faceUpSprite;
    }
    
    public void Hide()
    {
        spriteRenderer.sprite = faceDownSprite;
    }

    // Update is called once per frame
    void Update()
    {
        if (lerping)
        {

            lerpTime += Time.deltaTime;
            if (lerpTime > lerpDuration) lerpTime = lerpDuration;
            transform.position = Vector3.Lerp(startTransform, targetTransform, lerpTime / lerpDuration);

            if (lerpTime >= lerpDuration)
            {
                lerping = false;
                moveable = true;
                lerpTime = 0;
                if (destroyWhenLerpComplete)
                {
                    Destroy(gameObject);
                }
            }
        }

        if (scaleLerping)
        {
            scaleLerpTime += Time.deltaTime;
            if (scaleLerpTime > scaleLerpDuration) scaleLerpTime = scaleLerpDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, scaleLerpTime / scaleLerpDuration);
            
            if (scaleLerpTime >= scaleLerpDuration)
            {
                scaleLerping = false;
                scaleLerpTime = 0;
            }
        }
    }

    /**
     * Linearly interpolates the card to the specified position
     * This will tell the method in Update() to start lerping
     * Note: This uses global coordinates
     */
    public void TransformLerp(Vector3 endPosition)
    {
        startTransform = transform.position;
        targetTransform = endPosition;
        lerping = true;
        moveable = false;
    }

    public void ScaleLerp(Vector3 endScale)
    {
        startScale = transform.localScale;
        targetScale = endScale;
        scaleLerping = true;
    }

    public void SetText(string str)
    {
        if (!playText) return;
        playText.GetComponent<TextMeshProUGUI>().SetText(str);
    }

    /**
     * Whether you can drag this card (disabled when card is automatically moving)
     */
    public bool CanMove()
    {
        return moveable;
    }
    
    public bool HasBeenPlayed()
    {
        return hasBeenPlayed;
    }
    
    public void SetHasBeenPlayed(bool played)
    {
        hasBeenPlayed = played;
    }

    public Player GetPlayer()
    {
        return player;
    }
    
    public Player SetPlayer(Player newPlayer)
    {
        player = newPlayer;
        return player;
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public void SetId(string newId)
    {
        id = newId;
    }
    
    public string GetId()
    {
        return id;
    }
    
    public void SetCardName(string str)
    {
        cardName = str;
        if (descriptionText) descriptionText.SetText(cardName);
    }
    
    public string GetCardName()
    {
        return cardName;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }
    
    public void SetDestroyWhenLerpComplete(bool destroy)
    {
        destroyWhenLerpComplete = destroy;
    }
    
    public bool IsStarCard()
    {
        return starCard;
    }
    
    public bool IsPlayable()
    {
        return playable;
    }
    
    /**
     * Makes this card unplayable for the rest of this turn, but enables it next turn.
     */
    public void SetUnplayableThisTurn()
    {
        if (!playable) return;
        playable = false;
        if (xText) xText.SetActive(true);
        Player.TurnStartEvent += OnNewTurnSetPlayable;
    }

    /**
     * Subscriber method that re-enables card
     */
    private void OnNewTurnSetPlayable(Player p)
    {
        if (p != player) return;
        Player.TurnStartEvent -= OnNewTurnSetPlayable;
        if (xText) xText.SetActive(false);
        playable = true;
    }
    
    /**
     * Transfers this card to another player's hand
     */
    public void TransferToOtherPlayer(Player newPlayer)
    {
        if (player == newPlayer) return;
        player = newPlayer;
        transform.SetParent(player.GetPlayerHand().transform, true);
        player.GetPlayerHand().UpdateCardLocations();
    }
    
    public bool IsHighlighted()
    {
        return highlighted;
    }
    
    // todo is there a way to do this without this boolean, perhaps a modifier system?
    private bool scaledAlready;
    public void SetHighlighted(bool highlight)
    {
        highlighted = highlight;
        if (highlighted)
        {
            if (scaledAlready) return;
            transform.localScale += new Vector3(0.2f, 0.2f, 0);
            scaledAlready = true;
        }
        else
        {
            scaledAlready = false;
            transform.localScale -= new Vector3(0.2f, 0.2f, 0);
        }
    }
    
    public void SetStacks(int newStacks)
    {
        stacks = newStacks;
        if (stacks < 1) stacks = 1;
        if (stacks > 1 && stackText)
        {
            stackText.SetActive(true);
            stackText.GetComponent<TextMeshProUGUI>().SetText("x" + stacks);
        }
        else if (stackText)
        {
            stackText.SetActive(false);
        }
    }
    
    public int GetStacks()
    {
        return stacks;
    }

    /**
     * Removes the specified number of cards from this stack and returns a new Card with that amount
     */
    public Card Split(int amount)
    {
        if (amount >= stacks)
        {
            Game.Log("Cannot split more cards than are in the stack!");
            return null;
        }
        SetStacks(stacks - amount);
        
        GameObject newCardObj = Instantiate(gameObject, transform.position, transform.rotation);
        Card newCard = newCardObj.GetComponent<Card>();
        newCard.SetStacks(amount);
        return newCard;
    }
    
    // todo If you quickly mouse over a card multiple times it wiggles extra times
    public void Wiggle()
    {
        if (animator) animator.SetTrigger("Wiggle");
    }
    
    private void PlayStarCardEffect()
    {
        if (!starCardEffectPrefab) return;
        GameObject obj = Instantiate(starCardEffectPrefab, transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
    }

    /**
     * Checks both if the card itself has been disabled and if the play behavior allows playing
     */
    public bool CanPlay()
    {
        return playBehavior.CanPlay() && playable;
    }
    
    public void DisplayDescription(bool b)
    {
        cardDescription.SetActive(b);    
    }
}
