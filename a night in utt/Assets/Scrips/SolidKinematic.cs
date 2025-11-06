using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SolidKinematic : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        // Aseguramos que el Rigidbody sea Kinematic
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // Collider ya bloquea a otros objetos dinámicos automáticamente
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Detecta cualquier objeto que choque
        Debug.Log("Objeto colisionó con la barrera: " + collision.gameObject.name);
    }
}
