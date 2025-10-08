using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;

public class DerrotaManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text textoTiempo;

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private string userId;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser != null ? auth.CurrentUser.UserId : "guest";

        // 🔹 Mostrar el tiempo guardado desde GameData
        if (textoTiempo != null)
        {
            textoTiempo.text = $"Sobreviviste: {GameData.formattedTime}";
            Debug.Log($"Tiempo mostrado en derrota: {GameData.formattedTime}");
        }
        else
        {
            Debug.LogWarning("No se asignó textoTiempo en el inspector");
        }

        // ❌ Ya NO guardamos aquí en Firestore, solo mostramos el dato
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
