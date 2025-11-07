using UnityEngine;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    public float speed = 5f; // Velocidad de movimiento

    // Límites del mapa
    private float minX = -8.40f;
    private float maxX = 30.5f;
    private float minY = -4.8f;
    private float maxY = 20.15f;

    // 📍 Posición de reaparición al volver a la escena 1
    public static Vector3 spawnPosition = Vector3.zero;

    void Start()
    {
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Colisión con: " + collision.tag);

        // Si toca el trigger que lleva de la zona 1 a la 2
        if (collision.CompareTag("zona1a2"))
        {
            Debug.Log("Cambiando a mapa2...");
            SceneManager.LoadScene("mapa2"); // Cambia a la escena llamada "mapa2"
        }

        // Si toca el trigger que lleva de la zona 2 a la 1
        if (collision.CompareTag("zona2a1"))
        {
            Debug.Log("Cambiando a SampleScene...");

            // 📍 Posición donde aparecerá en SampleScene
            spawnPosition = new Vector3(28.7f, 19.75f, 0f); // <-- Cambia estas coordenadas

            SceneManager.LoadScene("SampleScene"); // Cambia a la escena llamada "SampleScene"
        }

        //colisiones del juego
        // 🏢 Si toca el trigger que lleva a la zona EdificioAPB (escenario 2)
        if (collision.CompareTag("delToro"))
        {
            SceneManager.LoadScene("delToroGame"); // Cambia a la escena llamada "EdificioAPB"
        }





        // 🏢 Si toca el trigger que lleva a la zona EdificioAPB (escenario 2)
        if (collision.CompareTag("zona2aAB"))
        {
            Debug.Log("Cambiando a EdificioAPB...");
            SceneManager.LoadScene("EdificioAPB"); // Cambia a la escena llamada "EdificioAPB"
        }


        // 🏢 Si toca el trigger que lleva a la zona EdificioAPB (escenario 2)
        if (collision.CompareTag("zonaABa2"))
        {

            Debug.Log("Cambiando a mapa2...");
            SceneManager.LoadScene("mapa2"); // Cambia a la escena llamada "mapa2"

            spawnPosition = new Vector3(10.3f, 11f, 0f); // <-- Cambia estas coordenadas

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
    }
}
