using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;
using UnityEngine.InputSystem;

public class PerformanceMonitor : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI performanceText;

    [Header("Update Settings")]
    [SerializeField] private float updateInterval = 0.5f;

    [Header("Display Options")]
    [SerializeField] private bool showFPS = true;
    [SerializeField] private bool showMemory = true;
    [SerializeField] private bool showCPU = true;
    [SerializeField] private bool showBattery = true;
    [SerializeField] private bool showResolution = true;
    [SerializeField] private bool showGraphicsInfo = true;

    // Performance variables
    private float deltaTime = 0.0f;
    private float fps = 0.0f;
    private float minFPS = float.MaxValue;
    private float maxFPS = 0.0f;
    private long totalMemory = 0;
    private long availableMemory = 0;
    private long usedMemory = 0;
    private float batteryLevel = 0.0f;
    private StringBuilder stringBuilder;



    // Coroutine reference
    private Coroutine updateCoroutine;

    void Start()
    {
        stringBuilder = new StringBuilder(500);

        // Create UI if not assigned
        if (performanceText == null)
        {
            CreateUI();
        }

        // Always start monitoring
        StartMonitoring();
    }

    void Update()
    {
        // Calculate FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        fps = 1.0f / deltaTime;

        // Track min/max FPS
        if (fps < minFPS) minFPS = fps;
        if (fps > maxFPS) maxFPS = fps;
    }

    void CreateUI()
    {
        // Create Canvas if it doesn't exist
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Performance Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create Text GameObject
        GameObject textGO = new GameObject("Performance Text");
        textGO.transform.SetParent(canvas.transform);

        performanceText = textGO.AddComponent<TextMeshProUGUI>();

        // Try to load the default font, fallback to TMP default if not found
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");
        if (font == null)
        {
            // Use TextMeshPro default font
            font = TMP_Settings.defaultFontAsset;
        }

        if (font != null)
        {
            performanceText.font = font;
        }

        performanceText.fontSize = 24;
        performanceText.color = Color.white;
        performanceText.alignment = TextAlignmentOptions.TopLeft;

        // Create background for better readability
        GameObject backgroundGO = new GameObject("Background");
        backgroundGO.transform.SetParent(textGO.transform);
        Image background = backgroundGO.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.7f);

        // Set positions and sizes
        RectTransform textRect = performanceText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0, 1);
        textRect.anchorMax = new Vector2(0, 1);
        textRect.pivot = new Vector2(0, 1);
        textRect.anchoredPosition = new Vector2(10, -10);
        textRect.sizeDelta = new Vector2(400, 300);

        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;

        // Move background behind text
        backgroundGO.transform.SetAsFirstSibling();
    }

    public void StartMonitoring()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        updateCoroutine = StartCoroutine(UpdatePerformanceInfo());
    }

    public void StopMonitoring()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }
    }

    IEnumerator UpdatePerformanceInfo()
    {
        while (true)
        {
            UpdateMemoryInfo();
            UpdateBatteryInfo();
            UpdateDisplayText();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void UpdateMemoryInfo()
    {
        // Use GC memory which is more reliable
        totalMemory = System.GC.GetTotalMemory(false);
        usedMemory = totalMemory;

        // Calculate available memory based on system RAM
        long systemMemoryBytes = (long)SystemInfo.systemMemorySize * 1024 * 1024;
        availableMemory = systemMemoryBytes - totalMemory;
    }

    void UpdateBatteryInfo()
    {
        batteryLevel = SystemInfo.batteryLevel;
    }

    void UpdateDisplayText()
    {
        stringBuilder.Clear();
        stringBuilder.AppendLine("=== PERFORMANCE MONITOR ===");

        if (showFPS)
        {
            stringBuilder.AppendLine($"FPS: {fps:F1}");
            stringBuilder.AppendLine($"Min FPS: {minFPS:F1} | Max FPS: {maxFPS:F1}");
            stringBuilder.AppendLine($"Frame Time: {deltaTime * 1000:F1} ms");
            stringBuilder.AppendLine();
        }

        if (showMemory)
        {
            stringBuilder.AppendLine($"Unity Memory: {FormatBytes(usedMemory)}");
            stringBuilder.AppendLine($"System Memory: {FormatBytes(totalMemory)}");
            stringBuilder.AppendLine($"Available: {FormatBytes(availableMemory)}");
            stringBuilder.AppendLine();
        }

        if (showCPU)
        {
            stringBuilder.AppendLine($"CPU: {SystemInfo.processorType}");
            stringBuilder.AppendLine($"CPU Cores: {SystemInfo.processorCount}");
            stringBuilder.AppendLine($"CPU Frequency: {SystemInfo.processorFrequency} MHz");
            stringBuilder.AppendLine();
        }

        if (showBattery)
        {
            string batteryStatus = SystemInfo.batteryStatus.ToString();
            if (batteryLevel >= 0)
            {
                stringBuilder.AppendLine($"Battery: {batteryLevel * 100:F0}% ({batteryStatus})");
            }
            else
            {
                stringBuilder.AppendLine($"Battery: N/A ({batteryStatus})");
            }
            stringBuilder.AppendLine();
        }

        if (showResolution)
        {
            stringBuilder.AppendLine($"Resolution: {Screen.width} x {Screen.height}");
            stringBuilder.AppendLine($"DPI: {Screen.dpi}");
            stringBuilder.AppendLine($"Refresh Rate: {Screen.currentResolution.refreshRate} Hz");
            stringBuilder.AppendLine();
        }

        if (showGraphicsInfo)
        {
            stringBuilder.AppendLine($"GPU: {SystemInfo.graphicsDeviceName}");
            stringBuilder.AppendLine($"VRAM: {SystemInfo.graphicsMemorySize} MB");
            stringBuilder.AppendLine($"Graphics API: {SystemInfo.graphicsDeviceType}");
            stringBuilder.AppendLine();
        }

        stringBuilder.AppendLine($"Device: {SystemInfo.deviceModel}");
        stringBuilder.AppendLine($"OS: {SystemInfo.operatingSystem}");
        stringBuilder.AppendLine($"RAM: {SystemInfo.systemMemorySize} MB");

        performanceText.text = stringBuilder.ToString();
    }

    string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024f:F1} KB";
        if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024f * 1024f):F1} MB";
        return $"{bytes / (1024f * 1024f * 1024f):F1} GB";
    }

    // Reset min/max FPS
    public void ResetFPSStats()
    {
        minFPS = float.MaxValue;
        maxFPS = 0.0f;
    }

    // Public methods for external control
    public void SetUpdateInterval(float interval)
    {
        updateInterval = Mathf.Max(0.1f, interval);
    }

    public float GetCurrentFPS()
    {
        return fps;
    }

    public long GetUsedMemory()
    {
        return usedMemory;
    }
}