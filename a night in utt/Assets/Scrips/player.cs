using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class player : MonoBehaviour
{
    public float speed = 5f;

    private float minX = -8.40f;
    private float maxX = 30.5f;
    private float minY = -4.8f;
    private float maxY = 20.15f;

    public static Vector3 spawnPosition = Vector3.zero;

    [Header("Diálogos")]
    public GameObject dialogo; // Diálogo genérico
    public GameObject[] dialogoJulie;
    public GameObject[] dialogoCharly;
    public GameObject[] dialogoNpc1;
    public GameObject[] dialogoJared;
    public GameObject[] dialogoNoc;
    public GameObject[] dialogoFat;
    public GameObject[] dialogoLuis;
    public GameObject[] dialogoChoche;
    public GameObject[] dialogoMark;
    public GameObject[] dialogoPunk;
    public GameObject[] dialogoAze;
    public GameObject[] dialogoVict;
    public GameObject[] dialogoP1;

    private string savePath;
    private string visitedTagsPath;
    private HashSet<string> visitedTags;

    private string visitCountPath;
    private int visitCount = 0;

    // Referencias a los scripts de desafío
    private desafio1 desafio1Ref;
    private desafio2 desafio2Ref;
    private desafio3 desafio3Ref;
    private desafio4 desafio4Ref;

    // Tags que van a ItemRule
    private string[] tagsItemRule = {
        "julie", "charly", "npc1", "jared", "noc",
        "fat", "luis", "choche", "mark", "punk",
        "aze", "vict","p1"
    };

    // Diccionario de diálogos por tag
    private Dictionary<string, GameObject[]> dialogosTags;

    void Start()
    {
        // 1. Inicializar rutas y datos locales
        savePath = Path.Combine(Application.persistentDataPath, "playerSave.json");
        visitCountPath = Path.Combine(Application.persistentDataPath, "visitCount.txt");

        if (File.Exists(visitCountPath))
            int.TryParse(File.ReadAllText(visitCountPath), out visitCount);

        // 2. Inicializar Diccionarios
        dialogosTags = new Dictionary<string, GameObject[]>()
        {
            {"julie", dialogoJulie},
            {"charly", dialogoCharly},
            {"npc1", dialogoNpc1},
            {"jared", dialogoJared},
            {"noc", dialogoNoc},
            {"fat", dialogoFat},
            {"luis", dialogoLuis},
            {"choche", dialogoChoche},
            {"mark", dialogoMark},
            {"punk", dialogoPunk},
            {"aze", dialogoAze},
            {"vict", dialogoVict},
            {"p1", dialogoP1}
        };

        // Spawn guardado
        if (spawnPosition != Vector3.zero)
        {
            transform.position = spawnPosition;
            spawnPosition = Vector3.zero;
        }

        // Buscar los desafíos en la escena
        string escena = SceneManager.GetActiveScene().name.ToLower();
        if (escena.Contains("game"))
        {
            desafio1Ref = FindFirstObjectByType<desafio1>();
            desafio2Ref = FindFirstObjectByType<desafio2>();
            desafio3Ref = FindFirstObjectByType<desafio3>();
            desafio4Ref = FindFirstObjectByType<desafio4>();
        }
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(moveX, moveY).normalized;
        transform.Translate(movement * speed * Time.deltaTime);

        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;

        // 🔹 Respuestas a los desafíos R1, R2, R3
        if (tag.StartsWith("R"))
        {
            string respuesta = tag;

            if (desafio1Ref != null) desafio1Ref.SetRespuestaJugador(respuesta);
            else if (desafio2Ref != null) desafio2Ref.SetRespuestaJugador(respuesta);
            else if (desafio3Ref != null) desafio3Ref.SetRespuestaJugador(respuesta);
            else if (desafio4Ref != null) desafio4Ref.SetRespuestaJugador(respuesta);

            // Cambiar al siguiente desafío
            // Ya no llamamos CambiarDesafio o SiguienteDesafio aquí abajo.
            // La lógica interna de SetRespuestaJugador (en desafioX.cs) se encarga de cambiarlo tras el feedback.

            // Volver al spawn
            transform.position = new Vector3(-7.251836f, -3.926194f, 0f);
            return;
        }

        // 🔹 Mostrar diálogos generales
        if (dialogosTags.ContainsKey(tag) && dialogosTags[tag] != null)
        {
            StartCoroutine(MostrarDialogosSecuenciales(dialogosTags[tag]));
        }

        // 🔹 ItemRule - SOLO validar si ya entró o no en la BD
        foreach (string itemTag in tagsItemRule)
        {
            if (tag == itemTag)
            {
                bool yaEntro = Sqlite.instance.IsStudentVisited(tag);

                if (yaEntro)
                {
                    // YA entró → SOLO muestra diálogos
                    if (dialogosTags.ContainsKey(tag))
                        StartCoroutine(MostrarDialogosSecuenciales(dialogosTags[tag]));
                    else if (dialogo != null)
                        StartCoroutine(MostrarDialogoTemporal(dialogo));

                    return;
                }
                else
                {
                    // Primera vez → entra a ruleta
                    SavePlayerPosition();
                    Sqlite.instance.SetStudentVisited(tag);
                    SceneManager.LoadScene("ItemRule");
                    return;
                }
            }
        }

        // 🔹 Cambios de escena
        switch (tag)
        {
            case "igmar":
                // Requisito: TODOS los items (0 a 11) y TODOS los maestros vencidos
                HandleIgmarEntry();
                break;

            case "zona1a2": SceneManager.LoadScene("mapa2"); break;
            case "zona2a1": spawnPosition = new Vector3(28.7f, 19.75f, 0f); SceneManager.LoadScene("SampleScene"); break;
            case "delToro": HandleJugadorConItems("delToro", Sqlite.instance.GetMasterRequiredItems("delToro"), "delToroGame"); break;
            case "ramiro": HandleJugadorConItems("ramiro", Sqlite.instance.GetMasterRequiredItems("ramiro"), "ramiroGame"); break;
            case "rosales": HandleJugadorConItems("rosales", Sqlite.instance.GetMasterRequiredItems("rosales"), "rosalesGame"); break;
            case "zona2aAB": SceneManager.LoadScene("EdificioAPB"); break;
            case "zonaABa2": spawnPosition = new Vector3(10.3f, 11f, 0f); SceneManager.LoadScene("mapa2"); break;
            case "zonaABaAA": SceneManager.LoadScene("EdificioAPA"); break;
            case "zonaABa3": SceneManager.LoadScene("mapa3"); break;
            case "mapa4aB": SceneManager.LoadScene("EdificioBPB"); break;
            case "zonaBBaBA": SceneManager.LoadScene("EdificioBPA"); break;
            case "zonaBBa4": SceneManager.LoadScene("mapa4"); break;
            case "zona4a5": SceneManager.LoadScene("mapa5"); break;
            case "zonaAAaAB": spawnPosition = new Vector3(18.3f, 20f, 0f); SceneManager.LoadScene("EdificioAPB"); break;
            case "zona3aAB": spawnPosition = new Vector3(27.35f, 18f, 0f); SceneManager.LoadScene("EdificioAPB"); break;
            case "zonaBBa3": spawnPosition = new Vector3(16.5f, 17f, 0f); SceneManager.LoadScene("mapa3"); break;
            case "zona4aBB": spawnPosition = new Vector3(20f, 18f, 0f); SceneManager.LoadScene("EdificioBPB"); break;
            case "zona5a4": spawnPosition = new Vector3(-7f, 11f, 0f); SceneManager.LoadScene("mapa4"); break;
            case "zonaBAaBB": spawnPosition = new Vector3(21f, -4f, 0f); SceneManager.LoadScene("EdificioBPB"); break;
        }
    }

    private void HandleIgmarEntry()
    {
        // 1. Verificar TODOS los items (0 al 11 => 12 items)
        List<int> collected = Sqlite.instance.GetActiveItemIndices();
        bool allItems = collected.Count >= 12; // Asumiendo que son 12 items totales

// 2. Verificar Maestros (DelToro, Ramiro, Rosales)
        bool allMasters = Sqlite.instance.AreAllMastersDefeated();

        // 3. Verificar items especificos de Igmar desde DB (deberían ser todos)
        int[] requiredIgmar = Sqlite.instance.GetMasterRequiredItems("igmar");
        bool hasAllItemsIgmar = true;
        foreach(int req in requiredIgmar)
        {
            if(!collected.Contains(req)) { hasAllItemsIgmar = false; break; }
        }

        Debug.Log($"[Igmar] Items DB check: {hasAllItemsIgmar}. Maestros Vencidos: {allMasters}");

        if (hasAllItemsIgmar && allMasters)
        {
            Debug.Log("[Igmar] ACCESO CONCEDIDO (Desafio Final).");
            SavePlayerPosition();
            SceneManager.LoadScene("igmarGame");
        }
        else
        {
            Debug.Log("[Igmar] Acceso Denegado. Faltan items o maestros.");
            if (dialogo != null) StartCoroutine(MostrarDialogoTemporal(dialogo));
        }
    }

    private void HandleJugadorConItems(string jugadorTag, int[] requiredItems, string escenaJuego)
    {
        // Usar Sqlite para verificar items obtenidos
        List<int> obtainedItems = Sqlite.instance.GetActiveItemIndices();
        
        string itemsStr = string.Join(",", obtainedItems);
        Debug.Log($"[{jugadorTag}] Items obtenidos: [{itemsStr}]. Requeridos: [{string.Join(",", requiredItems)}]");

        bool tieneTodos = true;
        foreach (int index in requiredItems)
        {
            if (!obtainedItems.Contains(index))
            {
                Debug.Log($"[{jugadorTag}] Falta item índice: {index}");
                tieneTodos = false;
                break;
            }
        }

        if (tieneTodos)
        {
            // Opcional: Validar si este maestro ya fue vencido para no dejar entrar de nuevo?
            // El usuario no especificó bloquear reentrada, solo "guardara si ganaste".
            // Dejamos entrar siempre si tiene items.
            Debug.Log($"[{jugadorTag}] Acceso concedido.");
            SavePlayerPosition();
            SceneManager.LoadScene(escenaJuego);
        }
        else if (dialogo != null)
        {
            Debug.Log($"[{jugadorTag}] Acceso denegado.");
            StartCoroutine(MostrarDialogoTemporal(dialogo));
        }
    }

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
    }

    private System.Collections.IEnumerator MostrarDialogoTemporal(GameObject dialogoObj, bool cargarItemRule = false)
    {
        dialogoObj.SetActive(true);
        yield return new WaitForSeconds(5f);
        dialogoObj.SetActive(false);

        if (cargarItemRule)
            SceneManager.LoadScene("ItemRule");
    }

    private System.Collections.IEnumerator MostrarDialogosSecuenciales(GameObject[] dialogos)
    {
        foreach (GameObject d in dialogos)
        {
            if (d != null)
            {
                d.SetActive(true);
                yield return new WaitForSeconds(5f);
                d.SetActive(false);
            }
        }
    }
}

// ----------------- CLASES AUXILIARES -----------------
[System.Serializable]
public class PlayerSaveData { public string sceneName; public float positionX, positionY, positionZ; }

[System.Serializable]
public class VisitedTagsData { public List<string> tags; }

[System.Serializable]
public class PlayerUsedItemsData { public List<int> usedIndices = new List<int>(); }
