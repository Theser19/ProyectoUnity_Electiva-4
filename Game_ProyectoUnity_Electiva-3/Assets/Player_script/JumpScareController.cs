using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JumpScareController : MonoBehaviour
{
    [Header("Jumpscare Settings")]
    public Image jumpscareImage; // La imagen del jumpscare (IMG_Jumpscare)
    public AudioClip jumpscareclip1; // El audio del susto
    public AudioSource jumpsource; // El Audio Source

    [Header("Timing")]
    public float jumpscareDisplayTime = 2f; // Tiempo que se muestra el jumpscare

    private bool hasTriggered = false; // Para evitar múltiples activaciones

    private void Start()
    {
        // Asegurar que la imagen esté oculta al inicio
        if (jumpscareImage != null)
        {
            jumpscareImage.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo activar una vez y cuando sea el jugador
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            TriggerJumpscare();
        }
    }

    private void TriggerJumpscare()
    {
        // Mostrar la imagen del jumpscare
        if (jumpscareImage != null)
        {
            jumpscareImage.enabled = true;
        }

        // Reproducir el sonido
        if (jumpsource != null && jumpscareclip1 != null)
        {
            jumpsource.PlayOneShot(jumpscareclip1);
        }

        // Iniciar corrutina para ocultar después del tiempo especificado
        StartCoroutine(CloseJumpscare());
    }

    private IEnumerator CloseJumpscare()
    {
        yield return new WaitForSeconds(jumpscareDisplayTime);

        // Ocultar la imagen del jumpscare
        if (jumpscareImage != null)
        {
            jumpscareImage.enabled = false;
        }
    }

    // Método para resetear el jumpscare (opcional)
    public void ResetJumpscare()
    {
        hasTriggered = false;
        if (jumpscareImage != null)
        {
            jumpscareImage.enabled = false;
        }
    }
}