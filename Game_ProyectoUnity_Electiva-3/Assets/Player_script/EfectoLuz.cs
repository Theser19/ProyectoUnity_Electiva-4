using UnityEngine;

public class EfectoLuz : MonoBehaviour // Cambiar la clase base a MonoBehaviour
{
    public int minIntensity;
    public int maxIntensity;
    public float  velocidad;

    private Light luz;

    void Start()
    {
        luz = GetComponent<Light>(); // Ahora GetComponent estará disponible
    }

    private void Update()
    {
        luz.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PingPong(Time.time * velocidad, 1));
    }
}
