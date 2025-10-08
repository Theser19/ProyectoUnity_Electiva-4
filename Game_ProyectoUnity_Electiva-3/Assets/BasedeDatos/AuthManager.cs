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
        // Esperar a que Firebase esté listo antes de autenticar
        if (FirebaseInit.IsReady())
        {
            Debug.Log("🔥 Firebase ya estaba listo, iniciando autenticación...");
            InitializeAuth();
        }
        else
        {
            Debug.Log("⏳ Esperando a que Firebase termine de inicializarse...");
            FirebaseInit.OnFirebaseReady += InitializeAuth;
        }
    }

    private void InitializeAuth()
    {
        FirebaseInit.OnFirebaseReady -= InitializeAuth; // evitar múltiples suscripciones
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            user = auth.CurrentUser;
            isAuthenticated = true;

            Debug.Log("✅ Usuario existente detectado.");
            Debug.Log($"   Email: {user.Email}");
            Debug.Log($"   UserID: {user.UserId}");
            Debug.Log($"   IsAnonymous: {user.IsAnonymous}");

            OnAuthCompleted?.Invoke();
        }
        else
        {
            Debug.Log("⚠️ No hay usuario logueado, iniciando sesión anónima...");
            SignInAnonymously();
        }
    }

    private void SignInAnonymously()
    {
        FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("❌ Error en autenticación anónima: " + task.Exception);
                Invoke(nameof(SignInAnonymously), 2f);
                return;
            }

            user = task.Result.User;
            isAuthenticated = true;

            Debug.Log("✅ Usuario anónimo autenticado exitosamente");
            Debug.Log($"   UserID: {user.UserId}");
            Debug.Log($"   IsAnonymous: {user.IsAnonymous}");

            OnAuthCompleted?.Invoke();
        });
    }

    public static bool IsReady()
    {
        return isAuthenticated && user != null;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
