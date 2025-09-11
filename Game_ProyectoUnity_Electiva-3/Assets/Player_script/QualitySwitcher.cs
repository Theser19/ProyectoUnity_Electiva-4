using UnityEngine;

public class QualitySwitcher : MonoBehaviour
{
    public float checkInterval = 10f; // cada cuánto segundos revisa el FPS
    private float timer;
    private int fps;

    void Update()
    {
        // Contador de tiempo
        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            fps = (int)(1f / Time.unscaledDeltaTime);
            AdjustQuality(fps);
            timer = 0f;
        }
    }

    void AdjustQuality(int fps)
    {
        if (fps < 25)
        {
            SetLow();
        }
        else if (fps < 45)
        {
            SetMid();
        }
        else
        {
            SetHigh();
        }
    }

    void SetLow()
    {
        QualitySettings.SetQualityLevel(0, true);
        Application.targetFrameRate = 30;
        Debug.Log("Calidad: BAJA");
    }

    void SetMid()
    {
        QualitySettings.SetQualityLevel(1, true);
        Application.targetFrameRate = 60;
        Debug.Log("Calidad: MEDIA");
    }

    void SetHigh()
    {
        QualitySettings.SetQualityLevel(2, true);
        Application.targetFrameRate = 60;
        Debug.Log("Calidad: ALTA");
    }
}
