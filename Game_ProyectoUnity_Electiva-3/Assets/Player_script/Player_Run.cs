using UnityEngine;
using SimpleInputNamespace;

public class Player_Run : MonoBehaviour
{
    public float velocidadCorrer = 10f;
    public float maxTiempoCorrer = 4f;
    public Control_Player control;
    private float velocidadNormal;
    private bool estaDescansando = false;
    public float tiempoCorriendo = 0f;
    public float tiempoDescanso = 0f;

    void Start()
    {
        control = GetComponent<Control_Player>();
        velocidadNormal = control.VelocidadNormal;
    }

    void Update()
    {
        bool botonCorrer = SimpleInput.GetButton("Fire3");

        // CAMBIO IMPORTANTE: Usar los mismos ejes que Control_Player
        bool estaMoviendose = SimpleInput.GetAxis("Horizontal") != 0 || SimpleInput.GetAxis("Vertical") != 0;

        if (estaDescansando)
        {
            tiempoDescanso += Time.deltaTime;
            control.velocidadActual = velocidadNormal;

            if (tiempoDescanso >= tiempoCorriendo * 0.4f)
            {
                estaDescansando = false;
                tiempoCorriendo = 0f;
                tiempoDescanso = 0f;
            }
        }
        else
        {
            if (botonCorrer && estaMoviendose)
            {
                control.velocidadActual = velocidadCorrer;
                tiempoCorriendo += Time.deltaTime;

                if (tiempoCorriendo >= maxTiempoCorrer)
                    estaDescansando = true;
            }
            else
            {
                control.velocidadActual = velocidadNormal;
            }
        }
    }
}