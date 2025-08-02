using UnityEngine;
using SimpleInputNamespace;

public class Crouch : MonoBehaviour
{
    public Animator Agacharse;
    [HideInInspector] public bool activo; // P�blico para que AgacharseUI pueda acceder

    void Start()
    {
        // Aseg�rate de que el Animator est� asignado en el inspector o usa esta l�nea:
        if (Agacharse == null)
            Agacharse = GetComponent<Animator>();
    }

    void Update()
    {
        // Usa correctamente SimpleInput sin Input delante
        if (SimpleInput.GetButtonDown("Fire2"))
        {
            CambiarEstadoAgacharse();
        }

        // Aplicar la animaci�n
        if (Agacharse != null)
            Agacharse.SetBool("Agacharse", activo);
    }

    public void CambiarEstadoAgacharse()
    {
        activo = !activo;
    }

    // M�todo p�blico para obtener el estado (�til para otros scripts)
    public bool EstaAgachado()
    {
        return activo;
    }
}