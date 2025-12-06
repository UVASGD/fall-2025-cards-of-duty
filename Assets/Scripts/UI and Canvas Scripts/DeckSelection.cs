using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelection : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string mainMenuSceneName;
    [SerializeField] public List<string> availableDecks;
    [SerializeField] public List<Sprite> deckIcons;
    
    [SerializeField] NextDeckButton player1DeckButton;
    [SerializeField] NextDeckButton player2DeckButton;
    [SerializeField] NextDeckButton cpuDeckButton;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        GameInformation.SetPlayer1Deck(player1DeckButton.deck);
        GameInformation.SetPlayer2Deck(player2DeckButton.deck);
        GameInformation.SetCpuDeck(cpuDeckButton.deck);
        
        try
        {
            SceneManager.LoadScene(gameSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game scene: {e.Message}\nPlease check if the scene name '{gameSceneName}' exists in your build settings.");
        }
    }
    
    public void ReturnToMainMenu()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogError("Main menu scene name is not set in the Credits script! Please set the scene name in the inspector.");
            return;
        }

        try
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load main menu scene: {e.Message}\nPlease check if the scene name '{mainMenuSceneName}' exists in your build settings.");
        }
    }
}
