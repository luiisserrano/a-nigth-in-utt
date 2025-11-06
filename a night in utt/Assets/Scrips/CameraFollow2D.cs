using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target; // El jugador (Player)
    public float smoothSpeed = 0.125f; // Suavidad del movimiento

    private Vector2 minPosition;
    private Vector2 maxPosition;

    void Start()
    {
        // Buscar el sprite del fondo automáticamente
        GameObject fondo = GameObject.Find("Fondo");
        if (fondo != null)
        {
            SpriteRenderer fondoRenderer = fondo.GetComponent<SpriteRenderer>();
            if (fondoRenderer != null)
            {
                // Obtener los límites del sprite
                Bounds bounds = fondoRenderer.bounds;

                // Calcular límites mínimos y máximos
                minPosition = bounds.min;
                maxPosition = bounds.max;
            }
            else
            {
                Debug.LogWarning("⚠️ El objeto 'Fondo' no tiene SpriteRenderer.");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró el objeto 'Fondo'. Verifica el nombre exacto en la jerarquía.");
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Posición deseada de la cámara (siguiendo al jugador)
            Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // Suavizado del movimiento
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Limitar el movimiento de la cámara dentro de los límites del fondo
            float camHalfHeight = Camera.main.orthographicSize;
            float camHalfWidth = camHalfHeight * Camera.main.aspect;

            float clampedX = Mathf.Clamp(smoothedPosition.x, minPosition.x + camHalfWidth, maxPosition.x - camHalfWidth);
            float clampedY = Mathf.Clamp(smoothedPosition.y, minPosition.y + camHalfHeight, maxPosition.y - camHalfHeight);

            transform.position = new Vector3(clampedX, clampedY, transform.position.z);
        }
    }
}
