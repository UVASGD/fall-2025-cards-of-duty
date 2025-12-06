using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NextDeckButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] public DeckSelection deckSelection;
    [SerializeField] public string deck;
    [SerializeField] public Image image;
    [SerializeField] public TextMeshProUGUI text;
    private int currentIndex = 0;
    void Start()
    {
        int deckCount = deckSelection.availableDecks.Count;
        
        deck = deckSelection.availableDecks[currentIndex];
        if (text != null)
        {
            text.text = deck;
        }
        Sprite selectedIcon = deckSelection.deckIcons[currentIndex];
        
        UpdateImage(selectedIcon);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        int deckCount = deckSelection.availableDecks.Count;
        currentIndex++;
        if (currentIndex >= deckCount)
        {
            currentIndex = 0;
        }
        
        deck = deckSelection.availableDecks[currentIndex];
        if (text != null)
        {
            text.text = deck;
        }
        Sprite selectedIcon = deckSelection.deckIcons[currentIndex];
        
        UpdateImage(selectedIcon);
    }
    
    public void UpdateImage(Sprite sprite)
    {
        if (image != null && sprite != null)
        {
            image.sprite = sprite;
        }
    }
    
}
