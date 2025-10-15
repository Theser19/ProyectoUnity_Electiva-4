using UnityEngine;
using TMPro;
using Firebase.Firestore;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Timer Settings")]
    public float currentTime = 0f;
    public bool isRunning = false;

    [Header("UI Elements")]
    public TMP_Text timerText;

    private FirebaseFirestore db;
    private FirebaseAuth auth;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        db = FirebaseFirestore.DefaultInstance;
        auth = FirebaseAuth.DefaultInstance;
    }

    void Start()
    {
        currentTime = 0f;
        isRunning = true;
        UpdateTimerUI();
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = GetFormattedTime();
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 🔹 Llamar este método cuando el jugador gane
    public async void DetenerYGuardarVictoria()
    {
        isRunning = false;

        if (!AuthManager.IsReady())
        {
            Debug.LogWarning("⚠️ Usuario no autenticado, no se guardará la victoria");
            return;
        }

        string userId = AuthManager.user.UserId;

        // 🔹 Congelamos los valores finales del tiempo
        float finalTime = currentTime;
        string formattedFinal = GetFormattedTime();

        // 🔹 Guardamos en GameData también
        GameData.finalTime = finalTime;
        GameData.formattedTime = formattedFinal;

        Debug.Log($"✅ Tiempo final guardado: {formattedFinal}");

        try
        {
            // Referencias a Firestore
            CollectionReference victoriasRef = db
                .Collection("Usuarios")
                .Document(userId)
                .Collection("Victorias");

            // 🔸 1. Guardar registro normal de victoria
            await victoriasRef.AddAsync(new
            {
                tiempoSegundos = finalTime,
                tiempoFormateado = formattedFinal,
                fecha = FieldValue.ServerTimestamp,
                causa = "victoria"
            });

            Debug.Log($"✅ Victoria guardada correctamente: {formattedFinal}");

            // 🔸 2. Guardar o actualizar documento “best”
            DocumentReference bestRef = victoriasRef.Document("best");
            DocumentSnapshot bestSnap = await bestRef.GetSnapshotAsync();

            if (bestSnap.Exists)
            {
                float bestTime = bestSnap.ContainsField("bestTime") ? bestSnap.GetValue<float>("bestTime") : float.MaxValue;

                if (finalTime < bestTime)
                {
                    await bestRef.SetAsync(new
                    {
                        bestTime = finalTime,
                        bestFormatted = formattedFinal
                    });
                    Debug.Log($"🏆 Nuevo mejor tiempo actualizado: {formattedFinal}");
                }
                else
                {
                    Debug.Log("⏱ No se superó el mejor tiempo anterior");
                }
            }
            else
            {
                await bestRef.SetAsync(new
                {
                    bestTime = finalTime,
                    bestFormatted = formattedFinal
                });
                Debug.Log($"🏁 Primer mejor tiempo guardado: {formattedFinal}");
            }

            // 🔹 Esperar 5 segundos antes de cambiar de escena
            Debug.Log("⏳ Esperando 5 segundos antes de cargar la escena final...");
            await Task.Delay(5000);

            SceneManager.LoadScene(3); // Cambiar a la escena de Victoria
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error al guardar victoria: {e.Message}");
        }
    }
}
