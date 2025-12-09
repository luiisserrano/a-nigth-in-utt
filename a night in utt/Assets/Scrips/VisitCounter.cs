using UnityEngine;
using System.IO;

public class VisitCounter : MonoBehaviour
{
    private string filePath;
    public int visitCount = 0;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "visitCount.txt");

        // Leer valor actual si existe
        if (File.Exists(filePath))
            int.TryParse(File.ReadAllText(filePath), out visitCount);

        // Incrementar solo aquí, en la escena "ganaste"
        visitCount++;
        File.WriteAllText(filePath, visitCount.ToString());

        Debug.Log("Visita número: " + visitCount);
    }
}
