using UnityEngine;
using UnityEngine.UI;

public class AgarrarObjetoUI : MonoBehaviour
{
    [Header("Referencias")]
    public Tomar_Objeto controladorAgarrar; // Referencia al controlador de agarrar objetos
    public RawImage imagenBoton; // La imagen del bot�n

    [Header("Estados Visuales del Bot�n")]
    public Color colorNormal = Color.white; // Color cuando no tiene objeto
    public Color colorConObjeto = Color.green; // Color cuando tiene objeto agarrado
    public Color colorObjetoDisponible = Color.yellow; // Color cuando hay objeto disponible para tomar

    [Header("Efectos Adicionales")]
    public bool usarEscala = true;
    public float escalaNormal = 1.0f;
    public float escalaConObjeto = 1.1f; // M�s grande cuando tiene objeto
    public float escalaObjetoDisponible = 1.05f; // Ligeramente m�s grande cuando hay objeto disponible

    [Header("Efectos de Transparencia")]
    public bool usarTransparencia = true;
    public float alfaNormal = 0.7f;
    public float alfaConObjeto = 1.0f;
    public float alfaObjetoDisponible = 0.9f;

    [Header("Efectos de Pulso (Opcional)")]
    public bool usarEfectoPulso = true;
    public float velocidadPulso = 3f; // Velocidad del efecto de pulso cuando tiene objeto
    public bool pulsoConObjeto = true; // Pulso cuando tiene objeto
    public bool pulsoObjetoDisponible = false; // Pulso cuando hay objeto disponible

    private Vector3 escalaOriginal;
    private Color colorOriginal;
    private bool estadoAnterior = false;
    private bool objetoDisponibleAnterior = false;

    void Start()
    {
        // Buscar referencias autom�ticamente si no est�n asignadas
        if (controladorAgarrar == null)
            controladorAgarrar = FindFirstObjectByType<Tomar_Objeto>();

        if (imagenBoton == null)
            imagenBoton = GetComponent<RawImage>();

        // Guardar valores originales
        escalaOriginal = transform.localScale;
        if (imagenBoton != null)
            colorOriginal = imagenBoton.color;

        // Aplicar estado inicial
        ActualizarEstadoVisual();
    }

    void Update()
    {
        if (controladorAgarrar == null) return;

        // Verificar estados actuales
        bool tieneObjeto = controladorAgarrar.HasObjectPickedUp();
        bool hayObjetoDisponible = TieneObjetoDisponible();

        // Verificar si cambi� alg�n estado
        if (tieneObjeto != estadoAnterior || hayObjetoDisponible != objetoDisponibleAnterior)
        {
            ActualizarEstadoVisual();
            estadoAnterior = tieneObjeto;
            objetoDisponibleAnterior = hayObjetoDisponible;
        }

        // Aplicar efectos de pulso
        if (usarEfectoPulso)
        {
            if (pulsoConObjeto && tieneObjeto)
            {
                AplicarEfectoPulso(colorConObjeto, escalaConObjeto, alfaConObjeto);
            }
            else if (pulsoObjetoDisponible && hayObjetoDisponible && !tieneObjeto)
            {
                AplicarEfectoPulso(colorObjetoDisponible, escalaObjetoDisponible, alfaObjetoDisponible);
            }
        }
    }

    void ActualizarEstadoVisual()
    {
        if (controladorAgarrar == null || imagenBoton == null) return;

        bool tieneObjeto = controladorAgarrar.HasObjectPickedUp();
        bool hayObjetoDisponible = TieneObjetoDisponible();

        // Determinar color y escala seg�n el estado
        Color nuevoColor;
        float nuevaEscala;
        float nuevoAlfa;

        if (tieneObjeto)
        {
            // Tiene objeto agarrado
            nuevoColor = colorConObjeto;
            nuevaEscala = escalaConObjeto;
            nuevoAlfa = alfaConObjeto;
        }
        else if (hayObjetoDisponible)
        {
            // Hay objeto disponible para tomar
            nuevoColor = colorObjetoDisponible;
            nuevaEscala = escalaObjetoDisponible;
            nuevoAlfa = alfaObjetoDisponible;
        }
        else
        {
            // Estado normal
            nuevoColor = colorNormal;
            nuevaEscala = escalaNormal;
            nuevoAlfa = alfaNormal;
        }

        // Aplicar transparencia
        if (usarTransparencia)
        {
            nuevoColor.a = nuevoAlfa;
        }

        imagenBoton.color = nuevoColor;

        // Cambiar escala
        if (usarEscala)
        {
            transform.localScale = escalaOriginal * nuevaEscala;
        }

        // Vibraci�n en m�vil cuando cambia de estado
#if UNITY_ANDROID || UNITY_IOS
        if (tieneObjeto != estadoAnterior)
        {
            Handheld.Vibrate();
        }
#endif
    }

    void AplicarEfectoPulso(Color colorBase, float escalaBase, float alfaBase)
    {
        if (imagenBoton == null) return;

        // Crear efecto de pulso
        float pulso = Mathf.Sin(Time.time * velocidadPulso) * 0.15f + 0.85f; // Oscila entre 0.7 y 1.0

        // Aplicar pulso al color
        Color colorConPulso = colorBase;
        colorConPulso.a = pulso * (usarTransparencia ? alfaBase : 1f);
        imagenBoton.color = colorConPulso;

        // Tambi�n aplicar pulso a la escala si est� activado
        if (usarEscala)
        {
            float escalaPulso = Mathf.Sin(Time.time * velocidadPulso) * 0.05f + escalaBase;
            transform.localScale = escalaOriginal * escalaPulso;
        }
    }

    // M�todo helper para verificar si hay objeto disponible
    private bool TieneObjetoDisponible()
    {
        if (controladorAgarrar == null) return false;

        // Usar el m�todo correcto del script principal
        return controladorAgarrar.HasObjectAvailable();
    }

    // M�todo p�blico para obtener el estado actual
    public string GetEstadoActual()
    {
        if (controladorAgarrar == null) return "Sin controlador";

        bool tieneObjeto = controladorAgarrar.HasObjectPickedUp();
        bool hayObjetoDisponible = TieneObjetoDisponible();

        if (tieneObjeto)
            return "Objeto agarrado: " + controladorAgarrar.GetPickedObject().name;
        else if (hayObjetoDisponible)
            return "Objeto disponible";
        else
            return "Normal";
    }

    // M�todo para cambiar colores desde el inspector en tiempo real
    [ContextMenu("Aplicar Cambios Visuales")]
    public void AplicarCambiosVisuales()
    {
        ActualizarEstadoVisual();
    }
}