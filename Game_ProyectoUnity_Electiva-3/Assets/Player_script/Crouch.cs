using UnityEngine;
using SimpleInputNamespace;

public class Crouch : MonoBehaviour
{
    public Animator Agacharse;
    [HideInInspector] public bool activo; // Público para que AgacharseUI pueda acceder

    void Start()
    {
        // Asegúrate de que el Animator esté asignado en el inspector o usa esta línea:
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

        // Aplicar la animación
        if (Agacharse != null)
            Agacharse.SetBool("Agacharse", activo);
    }

    public void CambiarEstadoAgacharse()
    {
        activo = !activo;
    }

    // Método público para obtener el estado (útil para otros scripts)
    public bool EstaAgachado()
    {
        return activo;
    }
}