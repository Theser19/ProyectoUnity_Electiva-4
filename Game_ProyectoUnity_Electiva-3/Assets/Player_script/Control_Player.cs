using UnityEngine;
using SimpleInputNamespace;

public class Control_Player : MonoBehaviour
{
    [Header("Configuración del Jugador")]
    public CharacterController controlador;
    public float Velocidad = 5f;
    public float gravedad = -9.81f;
    public float alturaSalto = 1f;

    [Header("Detección de Suelo")]

    public Transform EnelPiso;
    public float distanciaPiso;
    public LayerMask MascaraPiso;

    Vector3 velocidadabajo;
    bool EstaenelPiso;

    private void Update()
    {
        // Comprobar si está en el suelo
        EstaenelPiso = Physics.CheckSphere(EnelPiso.position, distanciaPiso, MascaraPiso);
        if (EstaenelPiso && velocidadabajo.y < 0)
        {
            velocidadabajo.y = -2f;  // Mantiene el personaje pegado al suelo
        }

        // Movimiento horizontal - USAR SOLO LOS EJES ESTÁNDAR DE MOVIMIENTO
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

        // Combinar ambos movimientos
        Vector3 movimientoTotal = (mover * Velocidad) + velocidadabajo;
        controlador.Move(movimientoTotal * Time.deltaTime);
    }
}