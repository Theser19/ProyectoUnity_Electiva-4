using Firebase;
using Firebase.Extensions;
using UnityEngine;
using System;

public class FirebaseInit : MonoBehaviour
{
    public static bool isInitialized = false;
    public static event Action OnFirebaseReady;

    [Header("Configuración")]
    [Tooltip("URL de tu base de datos de Firebase")]
    public string databaseUrl = "https://scapeforestfirebaseunity-default-rtdb.firebaseio.com";

    private void Awake()
    {
        // Asegurar que Firebase se inicialice primero
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("🔥 Iniciando Firebase...");
        InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            DependencyStatus dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                // Configurar la URL de la base de datos
                if (!string.IsNullOrEmpty(databaseUrl))
                {
                    app.Options.DatabaseUrl = new Uri(databaseUrl);
                    Debug.Log($"✅ Firebase inicializado con DatabaseURL: {databaseUrl}");
                }
                else
                {
                    Debug.LogWarning("⚠️ Firebase inicializado sin DatabaseURL");
                }

                isInitialized = true;

                // Notificar que Firebase está listo
                OnFirebaseReady?.Invoke();

                Debug.Log("✅ Firebase completamente inicializado y listo");
            }
            else
            {
                Debug.LogError($"❌ No se pudo inicializar Firebase: {dependencyStatus}");
                Debug.LogError($"   Detalles: {task.Exception}");

                // Reintentar después de 3 segundos
                Invoke(nameof(InitializeFirebase), 3f);
            }
        });
    }

    public static bool IsReady()
    {
        return isInitialized;
    }
}