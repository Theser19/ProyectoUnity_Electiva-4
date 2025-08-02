using UnityEngine;
using SimpleInputNamespace;

public class Control_Player : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    public CharacterController controlador;
    public float VelocidadNormal = 5f;
    public float gravedad = -9.81f;
    public float alturaSalto = 1f;

    [Header("Detección de Suelo")]
    public Transform EnelPiso;
    public float distanciaPiso;
    public LayerMask MascaraPiso;

    public float velocidadActual; // ← controlada por Player_Run
    Vector3 velocidadabajo;
    bool EstaenelPiso;

    private void Start()
    {
        velocidadActual = VelocidadNormal;
    }

    private void Update()
    {
        // Comprobar si está en el suelo
        EstaenelPiso = Physics.CheckSphere(EnelPiso.position, distanciaPiso, MascaraPiso);
        if (EstaenelPiso && velocidadabajo.y < 0)
        {
            velocidadabajo.y = -2f;
        }

        // Movimiento horizontal - usar los ejes estándar
        float x = SimpleInput.GetAxis("Horizontal");
        float z = SimpleInput.GetAxis("Vertical");
        Vector3 mover = transform.right * x + transform.forward * z;

        // Salto
        if (SimpleInput.GetButtonDown("Jump") && EstaenelPiso)
        {
            velocidadabajo.y = Mathf.Sqrt(alturaSalto * -2f * gravedad);
        }

        // Aplicar gravedad
        velocidadabajo.y += gravedad * Time.deltaTime;

        // CAMBIO IMPORTANTE: Usar velocidadActual en lugar de VelocidadNormal
        Vector3 movimientoTotal = (mover * velocidadActual) + velocidadabajo;
        controlador.Move(movimientoTotal * Time.deltaTime);
    }
}