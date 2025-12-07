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

    [SerializeField] private Sprite[] waterFrames;
    [SerializeField] Image waterRenderer;
    
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

    IEnumerator FadeOutBackground()
    {
        Image rawImage = GetComponent<Image>();
        Color originalColor = rawImage.color;
        float fadeDuration = 1f;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            rawImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
    
    void ShowButtons()
    {
        buttonCanvas.gameObject.SetActive(true);
        pressAnyButtonText.gameObject.SetActive(false);
    }

    public void PlayGame()
    {
        StartCoroutine(FadeOutBackground());
        StartCoroutine(WaterThenNext());
    }

    IEnumerator WaterThenNext()
    {
        RectTransform rt = waterRenderer.GetComponent<RectTransform>();

        // Anchor to full stretch
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;

        // Remove offsets
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        waterRenderer.gameObject.SetActive(true);
        // 15 frames, hardcoded
        for (int i = 0; i < 15; i++)
        {
            waterRenderer.sprite = waterFrames[i];
            if (i > 6)
            {
                // Start fading out
                Color color = waterRenderer.color;
                float alpha = Mathf.Lerp(1f, 0f, (i - 5) / 9f);
                waterRenderer.color = new Color(color.r, color.g, color.b, alpha);
            }
            yield return new WaitForSeconds(1/15f);
        }

        yield return new WaitForSeconds(0.5f);
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
