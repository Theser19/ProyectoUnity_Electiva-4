using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Firestore;

public static class GameResultSaver
{
    public static async Task GuardarVictoria(double tiempoSegundos, string tiempoFormateado)
    {
        if (!AuthManager.IsReady())
        {
            Debug.LogError("Usuario no autenticado, no se puede guardar la victoria.");
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        string userId = AuthManager.user.UserId;

        try
        {
            // -----------------------------
            // 1. Guardar registro en Partidas
            // -----------------------------
            DocumentReference partidaRef = db
                .Collection("Usuarios")
                .Document(userId)
                .Collection("Partidas")
                .Document();

            Dictionary<string, object> partidaData = new Dictionary<string, object>
            {
                { "fechaCompletado", Timestamp.FromDateTime(DateTime.UtcNow) },
                { "tiempo", tiempoSegundos },
                { "tiempoFormateado", tiempoFormateado }
            };

            await partidaRef.SetAsync(partidaData);
            Debug.Log($"✅ Victoria guardada en Partidas: {tiempoFormateado}");

            // -----------------------------
            // 2. Actualizar mejor tiempo del usuario
            // -----------------------------
            DocumentReference userRef = db.Collection("Usuarios").Document(userId);
            DocumentSnapshot userSnap = await userRef.GetSnapshotAsync();

            if (userSnap.Exists && userSnap.TryGetValue("mejorTiempo", out double mejorTiempoGuardado))
            {
                if (tiempoSegundos < mejorTiempoGuardado) // si es mejor, se actualiza
                {
                    await userRef.UpdateAsync(new Dictionary<string, object>
                    {
                        { "mejorTiempo", tiempoSegundos },
                        { "mejorTiempoFormateado", tiempoFormateado },
                        { "fechaMejorTiempo", Timestamp.FromDateTime(DateTime.UtcNow) }
                    });

                    Debug.Log($"🔥 Nuevo récord guardado: {tiempoFormateado}");
                }
            }
            else
            {
                // No existía, se guarda por primera vez
                await userRef.SetAsync(new Dictionary<string, object>
                {
                    { "mejorTiempo", tiempoSegundos },
                    { "mejorTiempoFormateado", tiempoFormateado },
                    { "fechaMejorTiempo", Timestamp.FromDateTime(DateTime.UtcNow) }
                }, SetOptions.MergeAll);

                Debug.Log($"🏅 Primer récord guardado: {tiempoFormateado}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error al guardar victoria: {e.Message}");
        }
    }

    public static async Task GuardarDerrota(double tiempoSegundos, string tiempoFormateado)
    {
        if (!AuthManager.IsReady())
        {
            Debug.LogError("Usuario no autenticado, no se puede guardar la derrota.");
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        string userId = AuthManager.user.UserId;

        try
        {
            // Guardar registro en Derrotas
            DocumentReference derrotaRef = db
                .Collection("Usuarios")
                .Document(userId)
                .Collection("Derrotas")
                .Document();

            Dictionary<string, object> derrotaData = new Dictionary<string, object>
            {
                { "fecha", Timestamp.FromDateTime(DateTime.UtcNow) },
                { "tiempo", tiempoSegundos },
                { "tiempoFormateado", tiempoFormateado }
            };

            await derrotaRef.SetAsync(derrotaData);
            Debug.Log($"☠️ Derrota guardada: {tiempoFormateado}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Error al guardar derrota: {e.Message}");
        }
    }
}
