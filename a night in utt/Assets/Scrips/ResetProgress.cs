using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ResetAndCounter : MonoBehaviour
{
    private string visitedTagsPath;
    private string playerSavePath;
    private string usedItemsPath;
    private string visitCountPath;

    public int visitCount = 0;

    void Start()
    {
        // Rutas de archivos
        visitedTagsPath = Path.Combine(Application.persistentDataPath, "playerVisitedTags.json");
        playerSavePath = Path.Combine(Application.persistentDataPath, "playerSave.json");
        usedItemsPath = Path.Combine(Application.persistentDataPath, "usedItems.json");
        visitCountPath = Path.Combine(Application.persistentDataPath, "visitCount.txt");

        // Leer y actualizar contador de visitas
        if (File.Exists(visitCountPath))
        {
            string content = File.ReadAllText(visitCountPath);
            int.TryParse(content, out visitCount);
        }

        visitCount++;
        File.WriteAllText(visitCountPath, visitCount.ToString());
        Debug.Log("Visita número: " + visitCount);
    }

    // Método para botón que borra archivos y reinicia escena
    public void ResetAndLoadSampleScene()
    {
        Debug.Log("=== NEW GAME PRESSED: Reiniciando archivos y contador ===");

        DeleteFileIfExists(visitedTagsPath, "Archivo de progreso borrado");
        DeleteFileIfExists(playerSavePath, "Archivo de posición borrado");
        DeleteFileIfExists(usedItemsPath, "Archivo de items usados borrado");
        DeleteFileIfExists(visitCountPath, "Archivo de visitas borrado");

        // Reiniciar contador
        visitCount = 0;
        Debug.Log("Contador de visitas reiniciado a 0");

        // Cargar escena SampleScene
        SceneManager.LoadScene("SampleScene");
    }

    private void DeleteFileIfExists(string path, string message)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"{message}: {path}");
        }
        else
        {
            Debug.Log($"No se encontró archivo: {path}");
        }
    }
}
