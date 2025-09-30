using UnityEngine;

public class Puerta : MonoBehaviour
{
    public float velocidad = 2f;
    public bool abrir = false;
    public bool verabrir = false;
    private Quaternion rotacionInicial;
    private Quaternion rotacionAbierta;
    private Quaternion objetivo;

    void Start()
    {
        rotacionInicial = transform.rotation;
        rotacionAbierta = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 80, 0));
        objetivo = rotacionInicial;
    }

    void Update()
    {
        // Rotar suavemente hacia el objetivo
        transform.rotation = Quaternion.RotateTowards(transform.rotation, objetivo, velocidad);

        // Input manual (opcional, puedes dejarlo o quitarlo)
        if (SimpleInput.GetButtonDown("e") && abrir)
        {
            if (!verabrir)
            {
                objetivo = rotacionAbierta;
                verabrir = true;
            }
            else
            {
                objetivo = rotacionInicial;
                verabrir = false;
            }
        }
    }

    // NUEVO: Método para abrir con item desde ItemUseZone
    public void AbrirConLlave()
    {
        if (!verabrir)
        {
            Debug.Log("Puerta abierta con llave!");
            objetivo = rotacionAbierta;
            verabrir = true;
        }
    }

    // OPCIONAL: Método para cerrar la puerta
    public void CerrarPuerta()
    {
        if (verabrir)
        {
            Debug.Log("Puerta cerrada");
            objetivo = rotacionInicial;
            verabrir = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            abrir = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            abrir = false;
        }
    }
}
