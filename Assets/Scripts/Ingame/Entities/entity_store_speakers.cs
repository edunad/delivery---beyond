

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class entity_store_speakers : MonoBehaviour {
    #region PRIVATE
        private AudioSource _audio;
    #endregion

    public void Awake() {
        this._audio = GetComponent<AudioSource>();
        this._audio.playOnAwake = false;
        this._audio.loop = false;

        if(CoreController.Instance != null) {
            CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
        }
    }

    public void setVolume(float vol) {
        this._audio.volume = vol;
    }

    public void setClip(AudioClip clip) {
        this._audio.clip = clip;
    }

    public void play() {
        this._audio.Play();
    }

    public void stop() {
        this._audio.Stop();
    }

    public void on() {
        this._audio.UnPause();

        util_fade_timer.fade(0.8f, 0f, 1f, (float val) => {
            if(this._audio == null) return;
            this._audio.pitch = val;
        }, (float val) => {
            this._audio.pitch = val;
        });
    }

    public void off() {
        util_fade_timer.fade(0.5f, 1f, 0f, (float val) => {
            if(this._audio == null) return;
            this._audio.pitch = val;
        }, (float val) => {
            this._audio.pitch = val;
            this._audio.Pause();
        });
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        if(status == GAMEPLAY_POWER_STATUS.HAS_POWER) this.on();
        else this.off();
    }

    public void OnDrawGizmos() {
        Gizmos.DrawIcon(this.transform.position, "gizmo_sndemitter");
    }
}
