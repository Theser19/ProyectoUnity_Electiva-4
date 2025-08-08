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

        // Input
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
