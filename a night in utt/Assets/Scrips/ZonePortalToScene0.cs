using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonePortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo si el objeto que entra tiene tag "player"
        if (collision.CompareTag("player"))
        {
            Debug.Log("Jugador entró al portal → Cargando escena ID 0 (SampleScene)");
            SceneManager.LoadScene(0); // Carga la escena con índice 0
        }
    }
}
