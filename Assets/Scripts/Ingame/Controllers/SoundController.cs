
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[DisallowMultipleComponent]
public class SoundController : MonoBehaviour {
	public static SoundController Instance;

	[Header("Settings")]
	public AudioMixer MasterMixer;
	public GameObject soundTemplate;

	private Queue<entity_sound> _soundPool = new Queue<entity_sound>();

    public SoundController() { Instance = this; }

    public void Play3DSound(AudioClip clip, Transform position, float pitch = 1f, float maxDistance = 10, float volume = 1f) {
        entity_sound snd = this.CreateSound();

        snd.SetClip(clip);
        snd.Set3DTarget(position, maxDistance);
        snd.SetPitch(pitch);
        snd.SetVolume(volume);

        snd.PlayOnce();
    }

    public void PlaySound(AudioClip clip, float pitch = 1f, float volume = 1f) {
        entity_sound snd = this.CreateSound();
        snd.SetClip(clip);
        snd.Set2D();
        snd.SetPitch(pitch);
        snd.SetVolume(volume);

        snd.PlayOnce();
    }

    public entity_sound CreateSound() {
        entity_sound snd = null;

        if (this._soundPool.Count <= 0) {
            GameObject obj = Instantiate<GameObject>(soundTemplate);
            obj.transform.parent = this.gameObject.transform;

            snd = obj.GetComponent<entity_sound>();
            if (snd == null)
                throw new UnityException("Invalid soundTemplate, missing entity_sound");
        } else {
            snd = this._soundPool.Dequeue();
        }

        return snd;
    }

    public void queueSound(entity_sound snd) {
        if(this._soundPool == null) return;

        snd.transform.parent = this.gameObject.transform;
        this._soundPool.Enqueue(snd);
    }
}
