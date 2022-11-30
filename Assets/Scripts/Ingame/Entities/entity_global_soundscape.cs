
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class entity_global_soundscape : MonoBehaviour {
    #region PRIVATE
        private AudioSource _ambient;
        private util_fade_timer _timer;
    #endregion

    public void Awake() {
        this._ambient = GetComponent<AudioSource>();
        this._ambient.playOnAwake = false;
        this._ambient.loop = true;
        this._ambient.volume = 0.05f;

        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        if(status == GAMEPLAY_POWER_STATUS.NO_POWER) {
            this._ambient.Play();

            if(this._timer != null) this._timer.stop();
            this._timer = util_fade_timer.fade(0.02f, 0.05f, 0.5f, (float volume) => {
                this._ambient.volume = volume;
            });
        }
        else {
            if(this._timer != null) {
                this._timer.stop();
                this._timer = null;
            }

            this._ambient.Stop();
            this._ambient.volume = 0.05f;
        }
    }
}
