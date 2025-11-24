using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class player : MonoBehaviour
{
    public static int charliyColision = 0;

    public float speed = 5f; // Velocidad de movimiento

    // Límites del mapa
    private float minX = -8.40f;
    private float maxX = 30.5f;
    private float minY = -4.8f;
    private float maxY = 20.15f;
    //---------------------------------------------------------
    private int item = 1;

    // 📍 Posición de reaparición al volver a la escena 1
    public static Vector3 spawnPosition = Vector3.zero;

    // 💬 Referencia al diálogo (asignar en el Inspector)
    [Header("Diálogo con Julie")]
    public GameObject dialogo;

    // Ruta del archivo de guardado
    private string savePath;

    void Start()
    {
        // Definir la ruta de guardado
        savePath = Path.Combine(Application.persistentDataPath, "playerSave.json");

        // Si hay una posición guardada, mover al jugador ahí
        if (spawnPosition != Vector3.zero)
        {
            transform.position = spawnPosition;
            spawnPosition = Vector3.zero; // limpiar para no repetir
        }
    }

    void Update()
    {
        // Movimiento del jugador (WASD o Flechas)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        transform.Translate(movement * speed * Time.deltaTime);

        // Limitar dentro de los bordes
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    // Método para guardar la posición y escena actual
    private void SavePlayerPosition()
    {
        PlayerSaveData saveData = new PlayerSaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z
        };

        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, jsonData);

        Debug.Log($"Posición guardada: {saveData.sceneName} - ({saveData.positionX}, {saveData.positionY}, {saveData.positionZ})");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colisión con: " + collision.tag);

        if (collision.CompareTag("zona1a2"))
        {
            Debug.Log("Cambiando a mapa2...");
            SceneManager.LoadScene("mapa2");
        }

        if (collision.CompareTag("zona2a1"))
        {
            Debug.Log("Cambiando a SampleScene...");
            spawnPosition = new Vector3(28.7f, 19.75f, 0f);
            SceneManager.LoadScene("SampleScene");
        }

        if (collision.CompareTag("delToro"))
        {
            if (item == 0)
            {
                if (dialogo != null)
                {
                    StartCoroutine(MostrarDialogoTemporal(dialogo));
                }
                else
                {
                    Debug.LogWarning("No se asignó el objeto 'dialogo' en el inspector del Player.");
                }
            }
            else
            {
                // Guardar posición antes de cambiar de escena
                SavePlayerPosition();
                SceneManager.LoadScene("delToroGame");
            }
        }

        if (collision.CompareTag("zona2aAB"))
        {
            Debug.Log("Cambiando a EdificioAPB...");
            SceneManager.LoadScene("EdificioAPB");
        }

        if (collision.CompareTag("zonaABa2"))
        {
            Debug.Log("Cambiando a mapa2...");
            SceneManager.LoadScene("mapa2");
            spawnPosition = new Vector3(10.3f, 11f, 0f);
        }

        if (collision.CompareTag("zonaABaAA"))
        {
            SceneManager.LoadScene("EdificioAPA");
        }

        if (collision.CompareTag("zonaABa3"))
        {
            SceneManager.LoadScene("mapa3");
        }

        if (collision.CompareTag("mapa4aB"))
        {
            SceneManager.LoadScene("EdificioBPB");
        }

        if (collision.CompareTag("zonaBBaBA"))
        {
            SceneManager.LoadScene("EdificioBPA");
        }

        if (collision.CompareTag("zonaBBa4"))
        {
            SceneManager.LoadScene("mapa4");
        }

        if (collision.CompareTag("zona4a5"))
        {
            SceneManager.LoadScene("mapa5");
        }

        if (collision.CompareTag("zonaAAaAB"))
        {
            SceneManager.LoadScene("EdificioAPB");
            spawnPosition = new Vector3(18.3f, 20f, 0f);
        }

        // 💬 Si colisiona con "julie", mostrar el diálogo temporalmente
        if (collision.CompareTag("julie"))
        {
            if (dialogo != null)
            {
                StartCoroutine(MostrarDialogoTemporal(dialogo));
            }
            else
            {
                Debug.LogWarning("No se asignó el objeto 'dialogo' en el inspector del Player.");
            }
        }

        if (collision.CompareTag("student"))
        {
            if (charliyColision == 0)
            {
                // Guardar posición antes de cambiar de escena
                SavePlayerPosition();
                SceneManager.LoadScene("ItemRule");
                charliyColision = 1;
            }
        }
    } // ← Esta llave cierra OnTriggerEnter2D (se había una llave extra)

    private System.Collections.IEnumerator MostrarDialogoTemporal(GameObject dialogo)
    {
        dialogo.SetActive(true); // mostrar
        yield return new WaitForSeconds(5f); // esperar 3 segundos
        dialogo.SetActive(false); // ocultar
    }
}

// Clase para los datos de guardado
[System.Serializable]
public class PlayerSaveData
{
    public string sceneName;
    public float positionX;
    public float positionY;
    public float positionZ;
}