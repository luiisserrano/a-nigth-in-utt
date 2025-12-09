using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void CerrarJuego()
    {
        Application.Quit();

        // Para que funcione dentro del editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
