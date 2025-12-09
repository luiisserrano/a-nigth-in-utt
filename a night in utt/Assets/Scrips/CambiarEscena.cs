using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    // Nombre de la escena a la que quieres ir
    public string nombreEscena = "SampleScene";

    // Esta función se asigna al botón OnClick
    public void IrAEscena()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}
