
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class entity_counter : MonoBehaviour {
    [Header("Settings")]
    public TextMeshPro text;

    private AudioSource _audioSource;
    private AudioClip[] _audioClips;

    private int _count = 0;

    public void Awake() {
        this._audioSource = GetComponent<AudioSource>();
        this._audioSource.playOnAwake = false;
        this._audioSource.volume = 0.45f;

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Counter/nextNumber")
        };
    }

    public void onButtonPress(entity_player ply) { this.nextNumber(); }

    public void nextNumber() {
        this._audioSource.clip = this._audioClips[0];
        this._audioSource.Play();

        this._count = Mathf.Clamp(this._count + 1, 0, 99);
        this.updateText();
    }

    private void updateText() {
        if(this._count < 10) this.text.text = "0" + this._count;
        else this.text.text = this._count.ToString();
    }
}
