using UnityEngine;
using System.Data;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class desafio2 : MonoBehaviour
{
    [Header("Imágenes del desafío")]
    public Sprite[] imagenes;
    private SpriteRenderer sr;

    public int imagenActual = -1;
    public string ultimaR = "";

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
    private int tiempoActual = 20;
    private Coroutine temporizadorCoroutine;

    [Header("UI Timer en pantalla")]
    public TextMeshProUGUI txtTimer;

    [Header("Texto de Bienvenida")]
    public TextMeshProUGUI textoBienvenida;
    public float duracionTexto = 5f;

    [Header("Contador de respuestas correctas")]
    private int correctas = 0;

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

    public void CambiarDesafio()
    {
        if (imagenes.Length == 0) return;

        imagenActual = Random.Range(0, imagenes.Length);
        sr.sprite = imagenes[imagenActual];

        Ajustar();
        ReiniciarTemporizador();
    }

    public void SetRespuestaJugador(string rTag)
    {
        ultimaR = rTag;

        if (temporizadorCoroutine != null)
            StopCoroutine(temporizadorCoroutine);

        ValidarRespuesta();
    }

    private void ValidarRespuesta()
    {
        if (Sqlite.instance == null) return;

        int idSQL = imagenActual + 1;

        // Aquí se consulta la unidad 2
        DataTable dt = Sqlite.instance.EjecutarConsulta(
            "SELECT respuesta FROM desafios2 WHERE id = " + idSQL
        );

        string correcta = dt.Rows[0]["respuesta"].ToString();

        if (ultimaR == correcta)
        {
            correctas++;
            MostrarCorrecto();

            if (correctas >= 5)
            {
                SceneManager.LoadScene("Ganaste");
                return;
            }
        }
        else
        {
            MostrarIncorrecto();
            QuitarVida();
        }
    }

    private void MostrarCorrecto()
    {
        imgCorrecto.SetActive(true);
        imgIncorrecto.SetActive(false);
        Invoke(nameof(SiguienteDesafio), 2f);
    }

    private void MostrarIncorrecto()
    {
        imgIncorrecto.SetActive(true);
        imgCorrecto.SetActive(false);
        Invoke(nameof(SiguienteDesafio), 2f);
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

    private void SiguienteDesafio()
    {
        OcultarMensajes();
        CambiarDesafio();
    }

    private void OcultarMensajes()
    {
        imgCorrecto.SetActive(false);
        imgIncorrecto.SetActive(false);
    }

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

        MostrarIncorrecto();
        QuitarVida();
        Invoke(nameof(SiguienteDesafio), 2f);
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

        transform.localScale = new Vector3(
            worldWidth / ancho,
            worldHeight / altura,
            1
        );
    }
}
