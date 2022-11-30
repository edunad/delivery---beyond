
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class entity_store_speakers : MonoBehaviour {

    [Header("Settings")]
    public AudioClip storeMusic;
    public AudioClip storeAnnouncement;

    #region PRIVATE
        private AudioSource _audio;
    #endregion

    public void Awake() {
        this._audio = GetComponent<AudioSource>();
        this._audio.clip = this.storeMusic;
        this._audio.playOnAwake = false;
        this._audio.loop = true;
        this._audio.Play();

        if(CoreController.Instance != null) {
            CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
            CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
        }
    }

    public void on() {
        this._audio.Play();

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
            this._audio.Stop();
        });
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        if(status == GAMEPLAY_POWER_STATUS.HAS_POWER) this.on();
        else this.off();
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        if(newStatus != GAMEPLAY_STATUS.WIN) return;
        this._audio.Stop();

        this._audio.volume = 1f;
        this._audio.clip = this.storeAnnouncement;
        this._audio.loop = false;
        this._audio.Play();
    }

    public void OnDrawGizmos() {
        Gizmos.DrawIcon(this.transform.position, "gizmo_sndemitter");
    }
}
