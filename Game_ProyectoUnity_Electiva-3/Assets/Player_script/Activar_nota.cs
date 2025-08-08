using UnityEngine;

public class Activar_nota : MonoBehaviour
{
    public GameObject notavisual;
    public bool activar;
    [HideInInspector] public bool notaActiva = false; // Ahora es público para que el UI pueda acceder

    private void Update()
    {
        if (SimpleInput.GetButtonDown("e") && activar == true)
        {
            if (notaActiva)
            {
                // Si está activa, la desactivamos
                notavisual.SetActive(false);
                notaActiva = false;
            }
            else
            {
                // Si está desactiva, la activamos
                notavisual.SetActive(true);
                notaActiva = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activar = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activar = false;
            notavisual.SetActive(false);
            notaActiva = false; // Resetear el estado
        }
    }
}