
using UnityEngine;
using VLB;

public class entity_light : MonoBehaviour {

    #region PRIVATE
        private Light _light;
        private AudioSource _audio;
        private VolumetricLightBeam _lightBeam;
        private ParticleSystem _lightParticles;
        private AudioClip[] _sndClip;
    #endregion

    public void Awake() {
        this._light = GetComponentInChildren<Light>(true);
        this._audio = GetComponentInChildren<AudioSource>(true);
        this._lightBeam = GetComponentInChildren<VolumetricLightBeam>(true);
        this._lightParticles = GetComponentInChildren<ParticleSystem>(true);

        this._sndClip = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Light/stalk_poweroff_off"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Light/stalk_poweroff_on")
        };

        if(CoreController.Instance != null) {
            CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
        }
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        if(status == GAMEPLAY_POWER_STATUS.HAS_POWER) this.on();
        else this.off();
    }

    public void off(bool random = true) {
        if(random) {
            util_timer.simple(Random.Range(0, 1f) , () => {
                this._off();
            });
        } else {
            this._off();
        }
    }

    public void on(bool random = true) {
        if(random) {
            util_timer.simple(Random.Range(0, 1f) , () => {
                this._on();
            });
        } else {
            this._on();
        }
    }

    private void _off() {
        this._light.enabled = false;
        this._lightBeam.enabled = false;

        this._lightParticles?.Stop();
        this._lightParticles?.Clear();
        this._audio?.Stop();
        SoundController.Instance.Play3DSound(this._sndClip[0], this.transform, Random.Range(0.75f, 1.25f), 10f, 0.5f);
    }

    private void _on() {
        this._light.enabled = true;
        this._lightBeam.enabled = true;
        this._lightParticles?.Play();

        this._audio?.Play();
        SoundController.Instance.Play3DSound(this._sndClip[1], this.transform, Random.Range(0.75f, 1.25f), 10f, 0.5f);
    }
}
