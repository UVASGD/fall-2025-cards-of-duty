using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName;
    [SerializeField] private string creditsSceneName;
    [SerializeField] private Canvas buttonCanvas;
    [SerializeField] private TextMeshProUGUI pressAnyButtonText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("MainMenu: Gameplay scene name is not set in the inspector!");
        }
        if (string.IsNullOrEmpty(creditsSceneName))
        {
            Debug.LogError("MainMenu: Credits scene name is not set in the inspector!");
        }

        StartCoroutine(FadeBackground());
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
        
        pressAnyButtonText.gameObject.SetActive(true);
        
        InputSystem.onAnyButtonPress
            .CallOnce(ctrl => ShowButtons());
    }
    void ShowButtons()
    {
        buttonCanvas.gameObject.SetActive(true);
        pressAnyButtonText.gameObject.SetActive(false);
    }

    public void PlayGame()
    {
        
        try
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MainMenu: Failed to load gameplay scene '{gameplaySceneName}'. Error: {e.Message}");
        }
    }

    public void GoToCredits()
    {
        try
        {
            SceneManager.LoadScene(creditsSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MainMenu: Failed to load credits scene '{creditsSceneName}'. Error: {e.Message}");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();

    }
}
