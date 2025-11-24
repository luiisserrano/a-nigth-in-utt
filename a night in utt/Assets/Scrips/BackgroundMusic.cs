using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public AudioSource audioSource;   // Arrastra aquí tu música

    public bool keepBetweenScenes = true; // Mantener música en todas las escenas
    public float volume = 0.5f;           // Volumen por defecto

    private void Awake()
    {
        if (keepBetweenScenes)
        {
            DontDestroyOnLoad(gameObject);

            // Evitar duplicados si ya existe otro con este script
            BackgroundMusic[] musics = FindObjectsOfType<BackgroundMusic>();
            if (musics.Length > 1)
            {
                Destroy(gameObject);
                return;
            }
        }

        SetupMusic();
    }

    private void SetupMusic()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("No asignaste un AudioSource, se creó uno automáticamente.");
        }

        audioSource.loop = true;
        audioSource.volume = volume;

        // Si no está sonando, la reproduce
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}
