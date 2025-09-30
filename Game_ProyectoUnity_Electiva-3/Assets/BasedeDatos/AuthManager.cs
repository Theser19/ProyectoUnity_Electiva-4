using Firebase.Auth;
using UnityEngine;
using System;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance;
    public static FirebaseUser user;
    public static bool isAuthenticated = false;

    // Evento para notificar cuando la autenticación esté lista
    public static event Action OnAuthCompleted;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log("🔐 Iniciando autenticación anónima...");
        SignInAnonymously();
    }

    void SignInAnonymously()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("❌ Autenticación anónima cancelada");
                // Reintentar después de 2 segundos
                Invoke(nameof(SignInAnonymously), 2f);
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("❌ Error en autenticación anónima: " + task.Exception);
                // Reintentar después de 2 segundos
                Invoke(nameof(SignInAnonymously), 2f);
                return;
            }

            // Autenticación exitosa
            AuthManager.user = task.Result.User;
            AuthManager.isAuthenticated = true;

            Debug.Log("✅ Usuario anónimo autenticado exitosamente");
            Debug.Log($"   UserID: {AuthManager.user.UserId}");
            Debug.Log($"   IsAnonymous: {AuthManager.user.IsAnonymous}");

            // Notificar a todos los sistemas que la autenticación está lista
            OnAuthCompleted?.Invoke();
        });
    }

    // Método público para verificar si está listo
    public static bool IsReady()
    {
        return isAuthenticated && user != null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}