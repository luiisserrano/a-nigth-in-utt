using UnityEngine;

public class AjustarFondo : MonoBehaviour
{
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Ajustar();
    }

    public void Ajustar()
    {
        if (sr.sprite == null) return;

        float altura = sr.sprite.bounds.size.y;
        float ancho = sr.sprite.bounds.size.x;

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Screen.width / Screen.height;

        transform.localScale = new Vector3(
            worldWidth / ancho,
            worldHeight / altura,
            1
        );
    }
}
