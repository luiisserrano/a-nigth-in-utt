using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public Image fadeImage; // Asigna aquí una imagen negra transparente (UI)
    public float fadeDuration = 1f; // Duración del fade en segundos
    public string sceneToLoad = "SampleScene"; // Nombre de la escena del juego

    public void PlayGame()
    {
        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        float elapsed = 0f;

        // Desvanecer (a negro)
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // Cargar la escena una vez terminado el fade
        SceneManager.LoadScene(sceneToLoad);
    }
}
