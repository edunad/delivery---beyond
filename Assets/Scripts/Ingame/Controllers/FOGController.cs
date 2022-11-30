
using UnityEngine;

[DisallowMultipleComponent]
public class FOGController : MonoBehaviour {
    public static FOGController Instance { get; private set; }

    [Header("Settings")]
    public float defaultFog = 0.27f;
    public float targetFog = 1f;
    public float changeSpeed = 0.5f;

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;

        RenderSettings.fogDensity = this.defaultFog;
        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    public void setFog(bool reverse) {
        RenderSettings.fogDensity = reverse ? this.targetFog : this.defaultFog;
        util_fade_timer.fade(this.changeSpeed, reverse ? this.targetFog : this.defaultFog, reverse ? this.defaultFog : this.targetFog, (float fog) => {
            RenderSettings.fogDensity = fog;
        });
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        this.setFog(status == GAMEPLAY_POWER_STATUS.HAS_POWER);
    }
}
