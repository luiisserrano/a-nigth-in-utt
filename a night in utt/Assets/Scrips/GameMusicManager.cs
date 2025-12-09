using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicManager : MonoBehaviour
{
    public static GameMusicManager instance;

    public AudioSource musicSource;

    // 🎶 Clips para cada escena
    public AudioClip menuClip;
    public AudioClip mapa2Clip;
    public AudioClip mapa3Clip;
    public AudioClip mapa4Clip;
    public AudioClip mapa5Clip;

    public AudioClip edificioAPAClip;
    public AudioClip edificioAPBClip;
    public AudioClip edificioBPAClip;
    public AudioClip edificioBPBClip;

    public AudioClip delToroClip;
    public AudioClip itemRuleClip;
    public AudioClip ganarClip;
    public AudioClip perderClip;

    public AudioClip igmarClip;
    public AudioClip ramiroClip;
    public AudioClip rosalesClip;

    public AudioClip sampleSceneClip;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name;
        Debug.Log("GameMusicManager → escena cargada: " + name);

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();

        switch (name)
        {
            case "Menu":
                Play(menuClip);
                break;
            case "mapa2":
                Play(mapa2Clip);
                break;
            case "mapa3":
                Play(mapa3Clip);
                break;
            case "mapa4":
                Play(mapa4Clip);
                break;
            case "mapa5":
                Play(mapa5Clip);
                break;
            case "EdificioAPA":
                Play(edificioAPAClip);
                break;
            case "EdificioAPB":
                Play(edificioAPBClip);
                break;
            case "EdificioBPA":
                Play(edificioBPAClip);
                break;
            case "EdificioBPB":
                Play(edificioBPBClip);
                break;
            case "delToroGame":
                Play(delToroClip);
                break;
            case "ItemRule":
                Play(itemRuleClip);
                break;
            case "ganaste":
                Play(ganarClip);
                break;
            case "perdiste":
                Play(perderClip);
                break;
            case "igmarGame":
                Play(igmarClip);
                break;
            case "ramiroGame":
                Play(ramiroClip);
                break;
            case "rosalesGame":
                Play(rosalesClip);
                break;
            case "SampleScene":
                Play(sampleSceneClip);
                break;
            default:
                // Por defecto no reproducir nada
                musicSource.Stop();
                break;
        }
    }

    private void Play(AudioClip clip)
    {
        if (musicSource == null) return;

        if (clip == null)
        {
            musicSource.Stop();
            musicSource.clip = null;
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
