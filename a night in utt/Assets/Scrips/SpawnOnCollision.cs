using UnityEngine;

public class SpawnOnCollision : MonoBehaviour
{
    [Header("Prefab que se mostrará al colisionar")]
    public GameObject prefab; // arrástralo desde Assets al Inspector

    [Header("Duración visible (segundos)")]
    public float duracion = 3f;

    [Header("Tag del jugador que activa")]
    public string tagJugador = "player"; // el tag del objeto que debe tocarlo

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tagJugador))
        {
            // Instanciar el prefab en la posición del objeto tocado
            Vector3 spawnPos = transform.position;
            GameObject instancia = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Destruirlo después de X segundos
            Destroy(instancia, duracion);
        }
    }
}
