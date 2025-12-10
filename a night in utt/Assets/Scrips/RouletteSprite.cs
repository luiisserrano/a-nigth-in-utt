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
        savePathPlayer = Path.Combine(Application.persistentDataPath, "playerSave.json");
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

        // 1. Girar la ruleta
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

        // 2. Selección de Item (Protegido contra fallos)
        bool success = false;
        try 
        {
            // --- Selección final evitando repetidos (Usando BDD) ---
            List<int> usedIndices = new List<int>();
            
            // Protección por si Sqlite falla o no existe
            if (Sqlite.instance != null)
            {
                usedIndices = Sqlite.instance.GetActiveItemIndices();
            }
            else
            {
                Debug.LogError("Sqlite instance es NULL en RouletteSprite.");
            }

            List<int> availableIndices = new List<int>();

            for (int i = 0; i < images.Length; i++)
            {
                if (!usedIndices.Contains(i))
                    availableIndices.Add(i);
            }

            if (availableIndices.Count == 0)
            {
                Debug.Log("Todos los items ya salieron. Seleccionando uno aleatorio (Visual).");
                int randomIndex = Random.Range(0, images.Length);
                spriteRenderer.sprite = images[randomIndex];
                // No guardamos nada
            }
            else
            {
                int finalIndex = availableIndices[Random.Range(0, availableIndices.Count)];
                spriteRenderer.sprite = images[finalIndex];

                // Guardar el item seleccionado en BDD
                if (Sqlite.instance != null)
                {
                    Sqlite.instance.UpdateItemActive(finalIndex + 1, true);
                    Debug.Log("Item obtenido: " + finalIndex + " (BD id: " + (finalIndex + 1) + ")");
                }
            }
            success = true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error crítico en RouletteSprite: " + ex.Message);
            // Si falla, mostramos una imagen cualquiera para no dejar vacía la ruleta
            spriteRenderer.sprite = images[Random.Range(0, images.Length)];
        }

        finished = true;
        
        // 3. Salida garantizada
        Debug.Log("Regresando a la escena guardada en 2 segundos...");
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

            player.spawnPosition = newPosition; // Asigna al static del player

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
