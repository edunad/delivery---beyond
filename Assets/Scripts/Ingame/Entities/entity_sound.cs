using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class entity_sound : MonoBehaviour {
	[HideInInspector]
	public AudioSource speaker;

	private float _timeRemaining = -1;
	private bool _playOnce;

	public void Awake() {
        this.speaker = GetComponent<AudioSource>();
        this.speaker.loop = false;
        this.speaker.playOnAwake = false;
        this.speaker.outputAudioMixerGroup = SoundController.Instance.MasterMixer.outputAudioMixerGroup;
	}

    public void SetClip(AudioClip clip) {
        this.Stop();
        this.speaker.clip = clip;
    }

    public void SetLooping(bool loop) {
        this.speaker.loop = loop;
    }

    public void SetParent(Transform parent) {
        this.speaker.transform.parent = parent;
        this.speaker.transform.localPosition = Vector3.zero;
    }

    public void Set3DTarget(Transform parent, float maxDistance = 10f, float minDistance = 1f) {
        this.speaker.spatialBlend = 0.45f; // 3d sound
        this.speaker.maxDistance = maxDistance;
        this.speaker.minDistance = minDistance;
        this.speaker.rolloffMode = AudioRolloffMode.Linear;

        this.SetParent(parent);
    }

    public void Set2D() {
        this.speaker.spatialBlend = 0f; // Play everywere
    }

    public void SetPitch(float pitch) {
        this.speaker.pitch = pitch;
    }


    public void PlayOnce() {
        if (this.speaker.clip == null) return;
        this._timeRemaining = Time.time + this.speaker.clip.length;
        this._playOnce = true;

        this.Stop();
        this.Play();
    }

    public void Stop() {
        this.speaker.Stop();
    }

    public void Pause() {
        this.speaker.Pause();
    }

    public void Play() {
        this.speaker.Play();
    }

    public void Destroy() {
        this.Stop();
        this.Set2D();
        this.SetClip(null);

        this._playOnce = false;
        SoundController.Instance.queueSound(this);
    }

    public void Update() {
        if (this.speaker == null || this.speaker.clip == null || !this._playOnce) return;
        if (Time.time < this._timeRemaining) return;
        this.Destroy();
    }

    public void OnDrawGizmos() {
        Gizmos.DrawIcon(this.transform.position, "gizmo_sndemitter");
    }
}
