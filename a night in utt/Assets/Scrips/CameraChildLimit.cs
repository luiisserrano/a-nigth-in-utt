using UnityEngine;

public class CameraChildLimit : MonoBehaviour
{
    public Transform player; // Asignar el Player
    private Camera cam;

    // Límites del fondo
    private float minX = -8.90f;
    private float maxX = 31.0f;
    private float minY = -5.3f;
    private float maxY = 20.65f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (player == null)
        {
            Debug.LogError("Player no asignado en la cámara.");
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Calcular mitad de la cámara
        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;

        // Posición deseada: centrar en el jugador
        float targetX = Mathf.Clamp(player.position.x, minX + halfWidth, maxX - halfWidth);
        float targetY = Mathf.Clamp(player.position.y, minY + halfHeight, maxY - halfHeight);

        // Actualizar posición de la cámara
        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }
}
