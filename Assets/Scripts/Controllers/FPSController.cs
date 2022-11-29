
using UnityEngine;

[DisallowMultipleComponent]
public class FPSController : MonoBehaviour {
    public static FPSController Instance { get; private set; }

    [Header("Settings")]
    public int targetFPS = 60;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;

        Debug.Log("FPS SET");

        QualitySettings.SetQualityLevel(3, true);
        QualitySettings.vSyncCount = 1;
    }
}
