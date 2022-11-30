
using System.Collections.Generic;
using UnityEngine;

public class entity_power_generator : MonoBehaviour {

    [Header("Settings")]
    public float restoreTime;

    [Header("LEDS")]
    public List<entity_led> LEDS = new List<entity_led>();

    #region PRIVATE
        private entity_switch _powerSwitch;
        private bool _powerNeedsReboot;
        private int _progress;
        private util_timer _timer;
        private AudioClip[] _audioClips;
    #endregion

    public void Awake() {
        this._powerSwitch = GetComponentInChildren<entity_switch>(true);
        this._powerSwitch.locked = true;

        this._powerSwitch.OnUSE += this.onSwitchPress;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/PowerGen/power_on"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/PowerGen/led_on"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/PowerGen/power_off")
        };

        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    private void setAllLEDS(bool active) {
        foreach(entity_led led in this.LEDS) led.setActive(active);
    }

    private void onSwitchPress(bool down) {
        if(!this._powerNeedsReboot) return;

        if(!down) {
            if(this._timer != null) {
                this._timer.stop();
                this._timer = null;
            }

            this._progress = 0;
            this.setAllLEDS(false);
        } else {
            if(this._timer != null) this._timer.stop();

            this._timer = util_timer.create(this.LEDS.Count, this.restoreTime, () => {
                if(this._progress < this.LEDS.Count) this.LEDS[this._progress].setActive(true);
                this._progress++;

                SoundController.Instance.Play3DSound(this._audioClips[1], this.transform, 0.75f, 3f, 0.8f);

                if(this._progress >= this.LEDS.Count) {
                    SoundController.Instance.Play3DSound(this._audioClips[0], this.transform, 1f, 8f, 0.8f);
                    CoreController.Instance.setPower(GAMEPLAY_POWER_STATUS.HAS_POWER);

                    this._powerNeedsReboot = false;
                    this.setAllLEDS(false);
                    return;
                }

            });
        }
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        bool power = status == GAMEPLAY_POWER_STATUS.HAS_POWER;
        if(!power) SoundController.Instance.Play3DSound(this._audioClips[2], this.transform, 0.75f, 5f, 1f);

        this._powerSwitch.locked = power;
        this._powerNeedsReboot = !power;
        this._progress = 0;
    }
}
