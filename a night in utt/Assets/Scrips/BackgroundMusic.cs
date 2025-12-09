using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public bool keepBetweenScenes = true;
    public float volume = 0.5f;

    private void Awake()
    {
        if (keepBetweenScenes)
        {
            DontDestroyOnLoad(gameObject);

            // Evitar duplicados
            BackgroundMusic[] musics = FindObjectsOfType<BackgroundMusic>();
            if (musics.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
        }

        SetupMusic();

        // 🔊 SUSCRIBIMOS EL EVENTO PARA DETENER LA MUSICA
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene scene, LoadSceneMode mode)
    {
        // 👉 AQUI DECIDES EN QUÉ ESCENA LA MÚSICA SE DEBE DETENER
        if (scene.name == "Ruleta" || scene.name == "Menu" || scene.name == "OtraEscena")
        {
            audioSource.Stop();
        }
    }

    private void SetupMusic()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.volume = volume;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
