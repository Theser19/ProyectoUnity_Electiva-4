using System.Collections;
using UnityEngine;

public class EfectoLuz_2: MonoBehaviour // Cambiar la clase base a MonoBehaviour
{
    public int minIntensity;
    public int maxIntensity;
    public float velocidad;


    private Light luz;
    private bool random;
    private bool loop = true;
    void Start()
    {
        luz = GetComponent<Light>(); // Ahora GetComponent estará disponible
        StartCoroutine(tiempo());
    }

    private void Update()
    {
        if(random)
        {
            luz.intensity = minIntensity;
        }
        else
        {
            luz.intensity = maxIntensity;
        }
    }

    IEnumerator tiempo()
     {
        while (loop)
        {
            yield return new WaitForSeconds(velocidad);
            random = !random;
        }
    }


}

