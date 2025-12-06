using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsCardShow : MonoBehaviour
{
    public Canvas targetCanvas;
    public Vector2 spawnAreaPaddingIn = new Vector2(100, 100);
    public Vector2 spawnAreaPaddingOut = new Vector2(50, 50);

    void Start()
    {
        StartCoroutine(SpawnRandomCard());
    }

    IEnumerator SpawnRandomCard()
    {
        while (true)
        {
            // Load all textures from Resources/Cards
            Texture2D[] cardTextures = Resources.LoadAll<Texture2D>("cards");

            if (cardTextures.Length == 0)
            {
                Debug.LogError("Somehow, there are no card textures.");
                yield return 0;
            }

            // Pick a random texture
            Texture2D selectedTexture = cardTextures[Random.Range(0, cardTextures.Length)];

            // Create a RawImage object
            GameObject cardObj = new GameObject("CardImage", typeof(RawImage));
            RawImage rawImage = cardObj.GetComponent<RawImage>();
            rawImage.texture = selectedTexture;
            rawImage.color = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 0);
            cardObj.transform.localScale = new Vector3(5, 5, 5);
            // Random rotation
            float randomRotation = Random.Range(0f, 360f);
            cardObj.transform.rotation = Quaternion.Euler(0, 0, randomRotation);

            // Parent it to the canvas
            cardObj.transform.SetParent(targetCanvas.transform, false);
            rawImage.SetNativeSize();

            // Set a random screen position
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaPaddingIn.x, Screen.width - spawnAreaPaddingOut.x),
                Random.Range(spawnAreaPaddingIn.y, Screen.height - spawnAreaPaddingOut.y)
            );

            // Convert screen position to canvas space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                targetCanvas.transform as RectTransform,
                randomPos,
                targetCanvas.worldCamera,
                out Vector2 canvasPos
            );

            rawImage.rectTransform.anchoredPosition = canvasPos;
            StartCoroutine(FadeInOut(cardObj));
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator FadeInOut(GameObject obj)
    {
        RawImage rawImage = obj.GetComponent<RawImage>();
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

        elapsedTime = 0f;
        yield return new WaitForSeconds(5);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            rawImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        Destroy(obj);
    }
}
