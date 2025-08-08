using UnityEngine;
using UnityEngine.UI;

public class AgarrarObjetoUI : MonoBehaviour
{
    [Header("Referencias")]
    public Tomar_Objeto controladorAgarrar; // Referencia al controlador de agarrar objetos
    public RawImage imagenBoton; // La imagen del botón

    [Header("Estados Visuales del Botón")]
    public Color colorNormal = Color.white; // Color cuando no tiene objeto
    public Color colorConObjeto = Color.green; // Color cuando tiene objeto agarrado
    public Color colorObjetoDisponible = Color.yellow; // Color cuando hay objeto disponible para tomar
    public Color colorNotaDisponible = Color.blue; // Color cuando hay nota disponible
    public Color colorPuertaDisponible = Color.orange; // Color cuando hay puerta disponible

    [Header("Efectos Adicionales")]
    public bool usarEscala = true;
    public float escalaNormal = 1.0f;
    public float escalaConObjeto = 1.1f; // Más grande cuando tiene objeto
    public float escalaObjetoDisponible = 1.05f; // Ligeramente más grande cuando hay objeto disponible

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
    private string estadoAnterior = "";
    private Activar_nota notaCercana;
    private Puerta puertaCercana;

    void Start()
    {
        // Buscar referencias automáticamente si no están asignadas
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

        DetectarInteractuables();

        string estadoActual = GetEstadoActual();

        // Verificar si cambió el estado
        if (estadoActual != estadoAnterior)
        {
            ActualizarEstadoVisual();
            estadoAnterior = estadoActual;

            // Vibración en móvil
#if UNITY_ANDROID || UNITY_IOS
            if (estadoAnterior != "Normal")
            {
                Handheld.Vibrate();
            }
#endif
        }

        // Aplicar efectos de pulso
        if (usarEfectoPulso)
        {
            bool tieneObjeto = controladorAgarrar.HasObjectPickedUp();
            bool hayObjetoDisponible = controladorAgarrar.HasObjectAvailable();

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

    void DetectarInteractuables()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // Detectar nota cercana
        notaCercana = null;
        Activar_nota[] notas = FindObjectsByType<Activar_nota>(FindObjectsSortMode.None);
        foreach (var nota in notas)
        {
            if (nota.activar)
            {
                notaCercana = nota;
                break;
            }
        }

        // Detectar puerta cercana - Por distancia
        puertaCercana = null;
        Puerta[] puertas = FindObjectsByType<Puerta>(FindObjectsSortMode.None);
        foreach (var puerta in puertas)
        {
            // Calcular distancia entre jugador y puerta
            float distancia = Vector3.Distance(player.transform.position, puerta.transform.position);

            // Distancia máxima para interactuar
            float distanciaMaxima = 3f; // 3 metros

            if (distancia <= distanciaMaxima)
            {
                puertaCercana = puerta;
                break;
            }
        }
    }

    void ActualizarEstadoVisual()
    {
        if (controladorAgarrar == null || imagenBoton == null) return;

        string estado = GetEstadoActual();

        // Determinar color y escala según el estado
        Color nuevoColor;
        float nuevaEscala;
        float nuevoAlfa;

        switch (estado)
        {
            case "Objeto agarrado":
                nuevoColor = colorConObjeto;
                nuevaEscala = escalaConObjeto;
                nuevoAlfa = alfaConObjeto;
                break;

            case "Objeto disponible":
                nuevoColor = colorObjetoDisponible;
                nuevaEscala = escalaObjetoDisponible;
                nuevoAlfa = alfaObjetoDisponible;
                break;

            case "Nota disponible":
                nuevoColor = colorNotaDisponible;
                nuevaEscala = escalaObjetoDisponible;
                nuevoAlfa = alfaObjetoDisponible;
                break;

            case "Puerta disponible":
                nuevoColor = colorPuertaDisponible;
                nuevaEscala = escalaObjetoDisponible;
                nuevoAlfa = alfaObjetoDisponible;
                break;

            default: // "Normal"
                nuevoColor = colorNormal;
                nuevaEscala = escalaNormal;
                nuevoAlfa = alfaNormal;
                break;
        }

        // Aplicar transparencia
        if (usarTransparencia)
        {
            nuevoColor.a = nuevoAlfa;
        }
        else
        {
            // Mantener alpha original si no se usa transparencia
            nuevoColor.a = imagenBoton.color.a;
        }

        imagenBoton.color = nuevoColor;

        // Cambiar escala
        if (usarEscala)
        {
            transform.localScale = escalaOriginal * nuevaEscala;
        }
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

        // También aplicar pulso a la escala si está activado
        if (usarEscala)
        {
            float escalaPulso = Mathf.Sin(Time.time * velocidadPulso) * 0.05f + escalaBase;
            transform.localScale = escalaOriginal * escalaPulso;
        }
    }

    // Método público para obtener el estado actual
    public string GetEstadoActual()
    {
        if (controladorAgarrar == null) return "Sin controlador";

        // Prioridad: Objeto agarrado > Objeto disponible > Nota > Puerta > Normal
        if (controladorAgarrar.HasObjectPickedUp())
        {
            return "Objeto agarrado";
        }

        if (controladorAgarrar.HasObjectAvailable())
        {
            return "Objeto disponible";
        }

        if (notaCercana != null)
        {
            return "Nota disponible";
        }

        if (puertaCercana != null)
        {
            return "Puerta disponible";
        }

        return "Normal";
    }

    public string GetInformacionDetallada()
    {
        string estado = GetEstadoActual();

        switch (estado)
        {
            case "Objeto agarrado":
                return "Presiona E para soltar: " + controladorAgarrar.GetPickedObject().name;

            case "Objeto disponible":
                return "Presiona E para tomar objeto";

            case "Nota disponible":
                if (notaCercana.notaActiva)
                    return "Presiona E para cerrar nota";
                else
                    return "Presiona E para leer nota";

            case "Puerta disponible":
                if (puertaCercana.verabrir)
                    return "Presiona E para cerrar puerta";
                else
                    return "Presiona E para abrir puerta";

            default:
                return "Explora para encontrar objetos";
        }
    }

    // Método para cambiar colores desde el inspector en tiempo real
    [ContextMenu("Aplicar Cambios Visuales")]
    public void AplicarCambiosVisuales()
    {
        ActualizarEstadoVisual();
    }
}