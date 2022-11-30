
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class SoundController : MonoBehaviour {
    public static SoundController Instance { get; private set; }

	[Header("Settings")]
	public AudioMixer musicMixer;
	public AudioMixer effectMixer;

	[Header("Template")]
	public GameObject soundTemplate;

    #region PRIVATE
	    private Queue<entity_sound> _soundPool = new Queue<entity_sound>();
    #endregion

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;

        OptionsController.Instance.OnSettingsUpdated += (string id, object val) => {
            if(id == "musicVolume") this.setMusicVolume((float) val);
            if(id == "effectsVolume") this.setEffectVolume((float) val);
        };
    }

    public void Start() {
        this.setMusicVolume(PlayerPrefs.GetFloat("musicVolume", 1f));
        this.setEffectVolume(PlayerPrefs.GetFloat("effectsVolume", 1f));
    }

    public entity_sound Play3DSound(AudioClip clip, Transform position, float pitch = 1f, float maxDistance = 10, float volume = 1f) {
        if(clip == null) return null;

        entity_sound snd = this.CreateSound();

        snd.SetMixer(this.effectMixer.FindMatchingGroups("Master")[0]);
        snd.SetClip(clip);
        snd.Set3DTarget(position, maxDistance);
        snd.SetPitch(pitch);
        snd.SetVolume(volume);

        snd.PlayOnce();

        return snd;
    }

    public entity_sound PlaySound(AudioClip clip, float pitch = 1f, float volume = 1f) {
        if(clip == null) return null;
        entity_sound snd = this.CreateSound();

        snd.SetMixer(this.musicMixer.FindMatchingGroups("Master")[0]);
        snd.SetClip(clip);
        snd.Set2D();
        snd.SetPitch(pitch);
        snd.SetVolume(volume);

        snd.PlayOnce();
        return snd;
    }

    public entity_sound CreateSound() {
        entity_sound snd = null;

        if (this._soundPool.Count <= 0) {
            GameObject obj = Instantiate<GameObject>(soundTemplate, Vector3.zero, Quaternion.identity, this.gameObject.transform);
            snd = obj.GetComponent<entity_sound>();
            if (snd == null)
                throw new UnityException("Invalid soundTemplate, missing entity_sound");
        } else {
            snd = this._soundPool.Dequeue();
        }

        snd.SetController(this);
        return snd;
    }

    public void queueSound(entity_sound snd) {
        if(this._soundPool == null) return;

        snd.transform.parent = this.gameObject.transform;
        this._soundPool.Enqueue(snd);
    }

    public void setEffectVolume(float vol) {
        float fixVol = Mathf.Log(Mathf.Clamp(vol, 0.001f, 1f)) * 20;
        this.effectMixer.SetFloat("volume", fixVol);
    }

    public void setMusicVolume(float vol) {
        float fixVol = Mathf.Log(Mathf.Clamp(vol, 0.001f, 1f)) * 20;
        this.musicMixer.SetFloat("volume", fixVol);
    }
}
