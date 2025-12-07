using System.Collections;
using System.Collections.Generic;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        StartCoroutine(FadeBackground());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator FadeBackground()
    {
        Image rawImage = GetComponent<Image>();
        Color originalColor = rawImage.color;
        float fadeDuration = 2f;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            rawImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }

    public void StartGame()
    {
        GameInformation.SetPlayer1Deck(player1DeckButton.deck);
        GameInformation.SetPlayer2Deck(player2DeckButton.deck);
        // GameInformation.SetCpuDeck(cpuDeckButton.deck);
        GameInformation.SetCpuDeck("cpu");
        
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
