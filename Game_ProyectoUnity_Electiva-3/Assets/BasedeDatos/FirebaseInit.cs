using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System;

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseApp App;
    public static FirebaseAuth Auth;
    public static FirebaseDatabase Database;

    public static bool IsInitialized = false;
    public static event Action OnFirebaseReady;

    [Header("Configuración")]
    [Tooltip("URL de tu base de datos de Firebase (Realtime Database)")]
    public string databaseUrl = "https://scapeforestfirebaseunity-default-rtdb.firebaseio.com";

    private void Awake()
    {
        // Asegurar que Firebase se inicialice primero y no se destruya entre escenas
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
            var dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                App = FirebaseApp.DefaultInstance;

                // Configurar la URL de la base de datos
                if (!string.IsNullOrEmpty(databaseUrl))
                {
                    App.Options.DatabaseUrl = new Uri(databaseUrl);
                    Database = FirebaseDatabase.GetInstance(App, databaseUrl);
                    Debug.Log($"✅ Firebase inicializado con DatabaseURL: {databaseUrl}");
                }
                else
                {
                    Debug.LogWarning("⚠️ Firebase inicializado sin DatabaseURL");
                }

                Auth = FirebaseAuth.DefaultInstance;
                IsInitialized = true;

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
        return IsInitialized;
    }
}
