using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JumpScareController : MonoBehaviour
{
    [Header("Jumpscare Settings")]
    public Image jumpscareImage;
    public AudioClip jumpscareclip1;
    public AudioSource jumpsource;

    [Header("Timing")]
    public float jumpscareDisplayTime = 2f;

    [Header("Scene Settings")]
    // public int escenaDerrota = 4;
    public float tiempoEsperaGuardado = 1f;

    private bool hasTriggered = false;
    private FirebaseFirestore db;

    private void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        if (jumpscareImage != null)
            jumpscareImage.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            TriggerJumpscare();
        }
    }

    private void TriggerJumpscare()
    {
        if (jumpscareImage != null)
            jumpscareImage.enabled = true;

        if (jumpsource != null && jumpscareclip1 != null)
            jumpsource.PlayOneShot(jumpscareclip1);

        if (GameTimer.Instance != null)
        {
            float tiempoSegundos = GameTimer.Instance.currentTime;
            string tiempoFormateado = GameTimer.Instance.GetFormattedTime();

            // ✅ Guardar tiempo global antes de cambiar de escena
            GameData.finalTime = tiempoSegundos;
            GameData.formattedTime = tiempoFormateado;

            StartCoroutine(GuardarYCambiarEscena(tiempoSegundos, tiempoFormateado));
        }
        else
        {
            Debug.LogWarning("GameTimer.Instance no encontrado");
            StartCoroutine(CambiarEscenaSinGuardar());
        }
    }

    IEnumerator GuardarYCambiarEscena(float tiempoSegundos, string tiempoFormateado)
    {
        GuardarTiempoDerrota(tiempoSegundos, tiempoFormateado);

        yield return new WaitForSeconds(jumpscareDisplayTime);

        if (jumpscareImage != null)
            jumpscareImage.enabled = false;

        yield return new WaitForSeconds(tiempoEsperaGuardado);

        Debug.Log($"Cambiando a escena de derrota (índice {"Derrota"})");
        SceneManager.LoadScene("Derrota");
    }

    IEnumerator CambiarEscenaSinGuardar()
    {
        yield return new WaitForSeconds(jumpscareDisplayTime);
        if (jumpscareImage != null)
            jumpscareImage.enabled = false;

        SceneManager.LoadScene("Derrota");
    }

    async void GuardarTiempoDerrota(float tiempoSegundos, string tiempoFormateado)
    {
        if (!AuthManager.IsReady())
        {
            Debug.LogWarning("Usuario no autenticado, no se guardará la derrota");
            return;
        }

        string userId = AuthManager.user.UserId;

        try
        {
            DocumentReference derrotaRef = db
                .Collection("Usuarios")
                .Document(userId)
                .Collection("Derrotas")
                .Document();

            Dictionary<string, object> derrotaData = new Dictionary<string, object>
            {
                { "tiempoSegundos", tiempoSegundos },
                { "tiempoFormateado", tiempoFormateado },
                { "fecha", Timestamp.GetCurrentTimestamp() },
                { "causa", "jumpscare" }
            };

            await derrotaRef.SetAsync(derrotaData);

            Debug.Log($"Derrota por jumpscare guardada: {tiempoFormateado}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al guardar derrota: {e.Message}");
        }
    }
}
