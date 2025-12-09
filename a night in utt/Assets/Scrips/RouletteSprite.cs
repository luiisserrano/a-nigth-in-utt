using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class UsedItemsData
{
    public List<int> usedIndices = new List<int>();
}

public class RouletteSprite : MonoBehaviour
{
    public Sprite[] images = new Sprite[12];
    public float speed = 0.5f;
    public float totalTime = 10f;

    public AudioSource audioSource; // Sonido del cambio de sprite

    private SpriteRenderer spriteRenderer;
    private bool finished = false;

    private string savePathItems;
    private string savePathPlayer;

    private UsedItemsData usedData;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        savePathItems = Path.Combine(Application.persistentDataPath, "usedItems.json");
        savePathPlayer = Path.Combine(Application.persistentDataPath, "playerSave.json");

        // Cargar los items usados si existe el archivo
        if (File.Exists(savePathItems))
        {
            string json = File.ReadAllText(savePathItems);
            usedData = JsonUtility.FromJson<UsedItemsData>(json);
        }
        else
        {
            usedData = new UsedItemsData();
        }
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

            if (audioSource != null)
                audioSource.Play();

            timer += speed;
            yield return new WaitForSeconds(speed);
        }

        // --- Selección final evitando repetidos ---
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < images.Length; i++)
        {
            if (!usedData.usedIndices.Contains(i))
                availableIndices.Add(i);
        }

        if (availableIndices.Count == 0)
        {
            Debug.LogWarning("Todos los items ya salieron. No hay más items disponibles.");
            finished = true;
            yield break; // Termina la ruleta sin seleccionar nada nuevo
        }

        int finalIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        spriteRenderer.sprite = images[finalIndex];

        // Guardar el item seleccionado
        usedData.usedIndices.Add(finalIndex);
        File.WriteAllText(savePathItems, JsonUtility.ToJson(usedData));

        finished = true;

        // Esperar 2 segundos antes de regresar a la escena
        yield return new WaitForSeconds(2f);
        ReturnToSavedScene();
    }

    private void ReturnToSavedScene()
    {
        if (File.Exists(savePathPlayer))
        {
            string jsonData = File.ReadAllText(savePathPlayer);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

            Vector3 newPosition = new Vector3(
                saveData.positionX + 1f,
                saveData.positionY + 1f,
                saveData.positionZ + 1f
            );

            player.spawnPosition = newPosition;

            Debug.Log($"Posición original: ({saveData.positionX}, {saveData.positionY}, {saveData.positionZ})");
            Debug.Log($"Regresando a escena: {saveData.sceneName} en posición: ({newPosition.x}, {newPosition.y}, {newPosition.z})");

            SceneManager.LoadScene(saveData.sceneName);
        }
        else
        {
            Debug.LogWarning("No se encontró archivo de guardado. Regresando a escena por defecto.");
            player.spawnPosition = Vector3.zero;
            SceneManager.LoadScene("SampleScene");
        }
    }
}
