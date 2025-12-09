using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class desafio4 : MonoBehaviour
{
    [Header("Imágenes por unidad (5 cada una)")]
    public Sprite[] unidad1 = new Sprite[5];
    public Sprite[] unidad2 = new Sprite[5];
    public Sprite[] unidad3 = new Sprite[5];

    private SpriteRenderer sr;
    private int imagenActual = -1;
    private string ultimaR = "";
    private int unidadActual = 1; // 1, 2 o 3

    [Header("UI de Resultado")]
    public GameObject imgCorrecto;
    public GameObject imgIncorrecto;

    [Header("UI de Vidas")]
    public GameObject vida1;
    public GameObject vida2;
    public GameObject vida3;
    private int vidasRestantes = 3;

    [Header("Temporizador")]
    public int tiempoTotal = 20;
    private int tiempoActual;
    private Coroutine temporizadorCoroutine;

    [Header("UI Timer en pantalla")]
    public TextMeshProUGUI txtTimer;

    [Header("Texto de Bienvenida")]
    public TextMeshProUGUI textoBienvenida;
    public float duracionTexto = 5f;

    [Header("Contador de respuestas correctas")]
    private int correctas = 0;

    // -------------------------------
    // Respuestas manuales para las primeras 5 preguntas
    // -------------------------------
    private string[] respuestasU1 = { "R1", "R2", "R1", "R3", "R2" };
    private string[] respuestasU2 = { "R1", "R1", "R3", "R3", "R1" };
    private string[] respuestasU3 = { "R1", "R3", "R2", "R2", "R1" };

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        imgCorrecto?.SetActive(false);
        imgIncorrecto?.SetActive(false);

        vida1.SetActive(true);
        vida2.SetActive(true);
        vida3.SetActive(true);

        if (txtTimer != null)
            txtTimer.text = tiempoTotal.ToString();
    }

    void Start()
    {
        if (textoBienvenida != null)
            StartCoroutine(MostrarTexto(textoBienvenida, duracionTexto));

        CambiarDesafio();
    }

    // -------------------------
    // Cambio de imagen / desafío
    // -------------------------
    public void CambiarDesafio()
    {
        // Escoger unidad aleatoria
        unidadActual = Random.Range(1, 4); // 1,2 o 3

        Sprite[] arreglo = unidad1;
        if (unidadActual == 2) arreglo = unidad2;
        if (unidadActual == 3) arreglo = unidad3;

        if (arreglo.Length == 0)
        {
            Debug.LogWarning("No hay imágenes en la unidad " + unidadActual);
            return;
        }

        // Solo las primeras 5 imágenes se consideran para validar manualmente
        imagenActual = Random.Range(0, Mathf.Min(5, arreglo.Length));
        sr.sprite = arreglo[imagenActual];

        Ajustar();
        ReiniciarTemporizador();
    }

    // -------------------------
    // Collider que recibe R1, R2 o R3
    // -------------------------
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag == "R1" || tag == "R2" || tag == "R3")
        {
            SetRespuestaJugador(tag);
        }
    }

    public void SetRespuestaJugador(string rTag)
    {
        ultimaR = rTag;

        if (temporizadorCoroutine != null)
            StopCoroutine(temporizadorCoroutine);

        ValidarRespuestaManual();
    }

    // -------------------------
    // Validar respuesta manual
    // -------------------------
    private void ValidarRespuestaManual()
    {
        string correcta = "";

        switch (unidadActual)
        {
            case 1:
                correcta = respuestasU1[imagenActual];
                break;
            case 2:
                correcta = respuestasU2[imagenActual];
                break;
            case 3:
                correcta = respuestasU3[imagenActual];
                break;
        }

        Debug.Log($"R colisionada: {ultimaR}, Respuesta correcta: {correcta}");

        if (ultimaR == correcta)
        {
            correctas++;
            StartCoroutine(MostrarResultadoCoroutine(imgCorrecto, imgIncorrecto, true));

            if (correctas >= 5)
            {
                SceneManager.LoadScene("Ganaste");
                return;
            }
        }
        else
        {
            StartCoroutine(MostrarResultadoCoroutine(imgIncorrecto, imgCorrecto, false));
            QuitarVida();
        }
    }

    private IEnumerator MostrarResultadoCoroutine(GameObject mostrar, GameObject ocultar, bool esCorrecto)
    {
        mostrar.SetActive(true);
        ocultar.SetActive(false);

        Debug.Log(esCorrecto ? "✅ Correcta" : "❌ Incorrecta");
        Debug.Log($"Activando imagen {(esCorrecto ? "Correcto" : "Incorrecto")} en Canvas");

        // Esperar un frame para que Unity renderice la UI
        yield return null;

        // Esperar 2 segundos antes de cambiar de desafío
        yield return new WaitForSeconds(2f);

        SiguienteDesafio();
    }

    private void QuitarVida()
    {
        vidasRestantes--;

        if (vidasRestantes == 2) vida3.SetActive(false);
        if (vidasRestantes == 1) vida2.SetActive(false);
        if (vidasRestantes == 0)
        {
            vida1.SetActive(false);
            SceneManager.LoadScene("Perdiste");
        }
    }

    public void SiguienteDesafio()
    {
        OcultarMensajes();
        CambiarDesafio();
    }

    private void OcultarMensajes()
    {
        imgCorrecto.SetActive(false);
        imgIncorrecto.SetActive(false);
    }

    // -------------------------
    // Temporizador en pantalla
    // -------------------------
    private void ReiniciarTemporizador()
    {
        if (temporizadorCoroutine != null)
            StopCoroutine(temporizadorCoroutine);

        tiempoActual = tiempoTotal;

        if (txtTimer != null)
            txtTimer.text = tiempoActual.ToString();

        temporizadorCoroutine = StartCoroutine(TimerRutina());
    }

    private IEnumerator TimerRutina()
    {
        while (tiempoActual > 0)
        {
            yield return new WaitForSeconds(1f);
            tiempoActual--;

            if (txtTimer != null)
                txtTimer.text = tiempoActual.ToString();
        }

        // Se acabó el tiempo
        StartCoroutine(MostrarResultadoCoroutine(imgIncorrecto, imgCorrecto, false));
        QuitarVida();
    }

    private IEnumerator MostrarTexto(TextMeshProUGUI texto, float duracion)
    {
        texto.gameObject.SetActive(true);
        texto.alpha = 1f;

        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            texto.alpha = Mathf.Lerp(1f, 0f, tiempo / duracion);
            yield return null;
        }

        texto.alpha = 0f;
        texto.gameObject.SetActive(false);
    }

    private void Ajustar()
    {
        if (sr.sprite == null) return;

        float altura = sr.sprite.bounds.size.y;
        float ancho = sr.sprite.bounds.size.x;

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Screen.width / Screen.height;

        transform.localScale = new Vector3(worldWidth / ancho, worldHeight / altura, 1);
    }
}
