using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using Firebase.Auth;
using System.Linq; // <- necesario para .Any() y .First()

public class VictoryManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text currentTimeText;
    public TMP_Text bestTimeText;
    public TMP_Text recordText; // opcional para "¡Nuevo récord!"

    private FirebaseFirestore db;
    private FirebaseAuth auth;
    private string userId;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
        userId = auth.CurrentUser != null ? auth.CurrentUser.UserId : "guest";

        string formattedTime = GameData.formattedTime;
        float rawTime = GameData.finalTime;

        if (currentTimeText != null)
            currentTimeText.text = "Tiempo actual: " + formattedTime;

        BuscarMejorTiempo(rawTime, formattedTime);
    }

    private void BuscarMejorTiempo(float newTime, string formattedTime)
    {
        var victoriasRef = db
            .Collection("Usuarios")
            .Document(userId)
            .Collection("Victorias");

        // OrderBy por "tiempoSegundos" (ascendente por defecto) y tomar 1 resultado
        victoriasRef
            .OrderBy("tiempoSegundos")
            .Limit(1)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogWarning("Error al consultar mejores tiempos: " + (task.Exception != null ? task.Exception.Message : "unknown"));
                    MostrarBest(formattedTime);
                    if (recordText != null) recordText.text = "";
                    return;
                }

                var snapshot = task.Result;
                if (snapshot != null && snapshot.Documents != null && snapshot.Documents.Any())
                {
                    var mejorDoc = snapshot.Documents.First();

                    // Intentamos obtener campos pero validamos que el tiempo sea > 0
                    float bestTime = 0f;
                    string bestFormatted = null;

                    bool hasTime = mejorDoc.TryGetValue<float>("tiempoSegundos", out float t);
                    bool hasFormatted = mejorDoc.TryGetValue<string>("tiempoFormateado", out string f);

                    if (hasTime && t > 0f)
                    {
                        bestTime = t;
                        bestFormatted = hasFormatted ? f : GetFormattedFromSeconds(t);
                        // comparar
                        if (newTime < bestTime)
                        {
                            MostrarBest(formattedTime);
                            if (recordText != null) recordText.text = "🎉 ¡Nuevo récord personal!";
                        }
                        else
                        {
                            MostrarBest(bestFormatted);
                            if (recordText != null) recordText.text = "";
                        }
                    }
                    else
                    {
                        // documento encontrado pero sin un tiempo válido > 0
                        MostrarBest(formattedTime);
                        if (recordText != null) recordText.text = "🏁 Primer tiempo registrado";
                    }
                }
                else
                {
                    // No hay documentos previos
                    MostrarBest(formattedTime);
                    if (recordText != null) recordText.text = "🏁 Primer tiempo registrado";
                }
            });
    }

    private void MostrarBest(string formattedTime)
    {
        if (bestTimeText != null)
            bestTimeText.text = "Mejor tiempo: " + formattedTime;
    }

    // Fallback: formatea segundos a mm:ss por si falta el campo formateado
    private string GetFormattedFromSeconds(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, secs);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
