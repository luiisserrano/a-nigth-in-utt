using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class RouletteSprite : MonoBehaviour
{
    public Sprite[] images = new Sprite[12];
    public float speed = 0.5f;
    public float totalTime = 10f;

    public AudioSource audioSource; // ← sonido del cambio

    private SpriteRenderer spriteRenderer;
    private bool finished = false;

    // Ruta del archivo de guardado
    private string savePath;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        savePath = Path.Combine(Application.persistentDataPath, "playerSave.json");
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RestartRoulette();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RestartRoulette();
    }

    private void RestartRoulette()
    {
        finished = false;
        StopAllCoroutines();
        StartCoroutine(Spin());
    }

    private System.Collections.IEnumerator Spin()
    {
        float timer = 0f;
        int index = 0;

        spriteRenderer.enabled = true;

        while (timer < totalTime)
        {
            if (finished) yield break;

            index = (index + 1) % images.Length;
            spriteRenderer.sprite = images[index];

            // 🔊 reproducir sonido por cambio de item
            if (audioSource != null)
                audioSource.Play();

            timer += speed;
            yield return new WaitForSeconds(speed);
        }

        // --- MOSTRAR EL SPRITE FINAL SIN FALLOS ---
        int finalIndex = Random.Range(0, images.Length);
        yield return null;
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = images[finalIndex];
        // -------------------------------------------

        finished = true;

        // 🔄 Regresar a la escena guardada después de un breve delay
        yield return new WaitForSeconds(2f); // Esperar 2 segundos para mostrar el resultado
        ReturnToSavedScene();
    }

    private void ReturnToSavedScene()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            // Usar la clase PlayerSaveData que ya existe en player.cs
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

            // Sumar 1 a cada coordenada para evitar colisiones
            Vector3 newPosition = new Vector3(
                saveData.positionX + 1f,
                saveData.positionY + 1f,
                saveData.positionZ + 1f
            );

            // Actualizar la posición guardada
            player.spawnPosition = newPosition;

            Debug.Log($"Posición original: ({saveData.positionX}, {saveData.positionY}, {saveData.positionZ})");
            Debug.Log($"Regresando a escena: {saveData.sceneName} en posición: ({newPosition.x}, {newPosition.y}, {newPosition.z})");

            // Cargar la escena guardada
            SceneManager.LoadScene(saveData.sceneName);
        }
        else
        {
            Debug.LogWarning("No se encontró archivo de guardado. Regresando a escena por defecto.");
            // Escena por defecto sin coordenadas específicas
            player.spawnPosition = Vector3.zero; // Esto hará que use la posición por defecto del Start()
            SceneManager.LoadScene("SampleScene");
        }
    }
}