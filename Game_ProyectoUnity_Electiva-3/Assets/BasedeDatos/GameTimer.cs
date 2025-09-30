using Firebase.Firestore;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textoTiempo;

    private float tiempoTranscurrido = 0f;
    private bool timerActivo = false;
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

        // Esperar a que el usuario esté autenticado antes de iniciar
        AuthManager.OnAuthCompleted += IniciarTimer;

        if (AuthManager.IsReady())
        {
            IniciarTimer();
        }
    }

    void OnDestroy()
    {
        AuthManager.OnAuthCompleted -= IniciarTimer;
    }

    void IniciarTimer()
    {
        timerActivo = true;
        tiempoTranscurrido = 0f;
        Debug.Log("⏱️ Cronómetro iniciado");
    }

    void Update()
    {
        if (timerActivo)
        {
            tiempoTranscurrido += Time.deltaTime;
            ActualizarUI();
        }
    }

    void ActualizarUI()
    {
        if (textoTiempo == null) return;

        TimeSpan timeSpan = TimeSpan.FromSeconds(tiempoTranscurrido);
        textoTiempo.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds);
    }

    public void DetenerYGuardar()
    {
        if (!timerActivo) return;

        timerActivo = false;
        Debug.Log($"⏱️ Tiempo final: {tiempoTranscurrido} segundos");

        GuardarTiempoEnFirebase();
    }

    async void GuardarTiempoEnFirebase()
    {
        if (!AuthManager.IsReady())
        {
            Debug.LogError("Usuario no autenticado. No se puede guardar el tiempo.");
            return;
        }

        string userId = AuthManager.user.UserId;

        // Formatear tiempo para legibilidad
        TimeSpan timeSpan = TimeSpan.FromSeconds(tiempoTranscurrido);
        string tiempoFormateado = string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds);

        try
        {
            DocumentReference partidaRef = db
                .Collection("Usuarios")
                .Document(userId)
                .Collection("Partidas")
                .Document();

            await partidaRef.SetAsync(new
            {
                tiempoSegundos = tiempoTranscurrido,
                tiempoFormateado = tiempoFormateado,
                fechaCompletado = FieldValue.ServerTimestamp,
                completado = true
            });

            Debug.Log($"☁️ Tiempo guardado en Firebase: {tiempoFormateado}");

            // También guardar el mejor tiempo del usuario
            await ActualizarMejorTiempo(userId, tiempoTranscurrido, tiempoFormateado);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al guardar tiempo: {e.Message}");
        }
    }

    async System.Threading.Tasks.Task ActualizarMejorTiempo(string userId, float tiempoActual, string tiempoFormateado)
    {
        try
        {
            DocumentReference userRef = db.Collection("Usuarios").Document(userId);
            DocumentSnapshot snapshot = await userRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                // Verificar si tiene un mejor tiempo previo
                if (snapshot.TryGetValue("mejorTiempo", out float mejorTiempoPrevio))
                {
                    if (tiempoActual < mejorTiempoPrevio)
                    {
                        // Nuevo récord personal
                        await userRef.UpdateAsync(new Dictionary<string, object>
                        {
                            { "mejorTiempo", tiempoActual },
                            { "mejorTiempoFormateado", tiempoFormateado },
                            { "fechaMejorTiempo", FieldValue.ServerTimestamp }
                        });

                        Debug.Log($"🏆 ¡Nuevo récord personal! {tiempoFormateado}");
                    }
                }
                else
                {
                    // Primera vez que completa el juego
                    await userRef.UpdateAsync(new Dictionary<string, object>
                    {
                        { "mejorTiempo", tiempoActual },
                        { "mejorTiempoFormateado", tiempoFormateado },
                        { "fechaMejorTiempo", FieldValue.ServerTimestamp }
                    });

                    Debug.Log($"🎉 Primera vez completado: {tiempoFormateado}");
                }
            }
            else
            {
                // Crear documento de usuario
                await userRef.SetAsync(new
                {
                    mejorTiempo = tiempoActual,
                    mejorTiempoFormateado = tiempoFormateado,
                    fechaMejorTiempo = FieldValue.ServerTimestamp
                });
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al actualizar mejor tiempo: {e.Message}");
        }
    }

    public float GetTiempoActual()
    {
        return tiempoTranscurrido;
    }

    public string GetTiempoFormateado()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(tiempoTranscurrido);
        return string.Format("{0:D2}:{1:D2}:{2:D2}",
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds);
    }

    public bool EstaActivo()
    {
        return timerActivo;
    }
}