using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Si ya existe otro MusicManager, destruye este
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Mantener este objeto al cambiar de escena
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
