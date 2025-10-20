using TMPro;
using UnityEngine;

public class DescriptionCanvas : MonoBehaviour
{

    [SerializeField] public Vector3 absoluteCardLocation;
    [SerializeField] public Vector3 cardScale = new Vector3(4, 4, 4);
    [SerializeField] public DragCards dragCardsSystem;
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public TextMeshProUGUI descriptionText;
    private Card targetCard;
    private Vector3 originalCardLocation;
    private Vector3 originalScale;
    private int originalSortingOrder;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(Card card)
    {
        if (!card) return;
        if (targetCard) return;

        titleText.text = card.GetCardName();
        descriptionText.text = card.GetCardDescription();
        gameObject.SetActive(true);
        targetCard = card;
        dragCardsSystem.disableCardClicks = true;
        originalSortingOrder = card.GetSpriteRenderer().sortingOrder;
        originalCardLocation = card.transform.position;
        originalScale = card.transform.localScale;
        
        card.GetSpriteRenderer().sortingOrder = 1001;
        card.TransformLerp(new Vector3(-9, 1, 0));
        card.ScaleLerp(new Vector3(4, 4, 4));
        
    }
    
    public void Deactivate()
    {
        targetCard.ScaleLerp(Vector3.one);
        dragCardsSystem.disableCardClicks = false;
        targetCard.TransformLerp(originalCardLocation);
        targetCard.ScaleLerp(originalScale);
        targetCard.GetSpriteRenderer().sortingOrder = originalSortingOrder;

        targetCard = null;
        gameObject.SetActive(false);
    }
}
