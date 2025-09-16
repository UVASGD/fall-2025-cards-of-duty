using CardBehaviors;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Card : MonoBehaviour
{
    // Define which player state this card belongs to
    protected Player player = null;
    private bool moveable = true;
    private bool playable = true;
    private bool hasBeenPlayed = false;
    private bool highlighted = false;

    private int stacks = 1;
    
    private bool lerping = true;
    private float lerpTime = 0;
    private const float lerpIncrement = 1 / 15f;
    private Vector3 startTransform = Vector3.zero;
    private Vector3 targetTransform = Vector3.zero;
    
    [SerializeField] private Sprite faceDownSprite;
    private Sprite faceUpSprite;
    private SpriteRenderer spriteRenderer;
    [FormerlySerializedAs("text")] [SerializeField] public GameObject playText;
    [SerializeField] public GameObject stackText;
    [SerializeField] public GameObject xText;
    private bool destroyWhenLerpComplete = false;
    
    private string id;
    [SerializeField] private bool starCard = false;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        faceUpSprite = spriteRenderer.sprite;
        animator = GetComponent<Animator>();
        if (playText) playText.SetActive(false);
        if (stackText) stackText.SetActive(false);
        if (xText) xText.SetActive(false);
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
            lerpTime += lerpIncrement;
            transform.position = Vector3.Lerp(startTransform, targetTransform, lerpTime);
            if (lerpTime >= 1)
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
    }

    // Uses global position
    public void TransformLerp(Vector3 endPosition)
    {
        startTransform = transform.position;
        targetTransform = endPosition;
        lerping = true;
        moveable = false;
    }

    public void SetText(string str)
    {
        if (!playText) return;
        playText.GetComponent<TextMeshProUGUI>().SetText(str);
    }

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

    private void OnNewTurnSetPlayable(Player p)
    {
        if (p != player) return;
        Player.TurnStartEvent -= OnNewTurnSetPlayable;
        if (xText) xText.SetActive(false);
        playable = true;
    }
    
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

    private bool scaledAlready;
    public void SetHighlighted(bool highlight)
    {
        // Game.Log(id + "highlighted: " + highlight);
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
    
    public void PlayStarCardEffect()
    {
        if (!starCardEffectPrefab) return;
        GameObject obj = Instantiate(starCardEffectPrefab, transform);
        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;
    }

    public bool CanPlay()
    {
        return playBehavior.CanPlay() && playable;
    }
}
