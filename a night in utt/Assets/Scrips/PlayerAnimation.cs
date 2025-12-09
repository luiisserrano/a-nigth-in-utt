using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public float speed = 5f;

    [Header("Sprites de movimiento")]
    public Sprite upIdle, upStep1, upStep2;
    public Sprite downIdle, downStep1, downStep2;
    public Sprite rightIdle, rightStep1, rightStep2;

    [Header("Audio")]
    public AudioSource stepSound; // Aquí va el AudioSource con tu sonido

    private SpriteRenderer sr;
    private float animationTimer = 0f;
    public float animationSpeed = 0.2f;
    private int currentFrame = 0;

    private enum Direction { Up, Down, Left, Right }
    private Direction lastDirection = Direction.Down;
    private Sprite lastIdle;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        lastIdle = downIdle;
        sr.sprite = lastIdle;
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        bool isMoving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f;

        // Movimiento
        Vector2 movement = new Vector2(moveX, moveY).normalized * speed * Time.deltaTime;
        transform.Translate(movement);

        // Dirección
        if (isMoving)
        {
            if (Mathf.Abs(moveX) > Mathf.Abs(moveY))
                lastDirection = moveX > 0 ? Direction.Right : Direction.Left;
            else
                lastDirection = moveY > 0 ? Direction.Up : Direction.Down;
        }

        // Seleccionar sprites según dirección
        Sprite idle = downIdle, step1 = downStep1, step2 = downStep2;
        bool flipX = false;

        switch (lastDirection)
        {
            case Direction.Up:    idle = upIdle; step1 = upStep1; step2 = upStep2; break;
            case Direction.Down:  idle = downIdle; step1 = downStep1; step2 = downStep2; break;
            case Direction.Right: idle = rightIdle; step1 = rightStep1; step2 = rightStep2; break;
            case Direction.Left:  idle = rightIdle; step1 = rightStep1; step2 = rightStep2; flipX = true; break;
        }

        sr.flipX = flipX;

        if (isMoving)
        {
            lastIdle = idle;

            animationTimer += Time.deltaTime;

            if (animationTimer >= animationSpeed)
            {
                animationTimer = 0f;

                currentFrame = (currentFrame + 1) % 2;

                // 🔊 Sonido limpio y sin pisarse
                if (stepSound != null)
                    stepSound.PlayOneShot(stepSound.clip);
            }

            sr.sprite = currentFrame == 0 ? step1 : step2;
        }
        else
        {
            sr.sprite = lastIdle;
        }
    }
}
