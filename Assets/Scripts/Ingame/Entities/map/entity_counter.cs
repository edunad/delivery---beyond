
using TMPro;
using UnityEngine;

public class entity_counter : MonoBehaviour {
    [Header("Settings")]
    public TextMeshPro text;

    #region PRIVATE
        private int _count = 0;
        private AudioClip[] _audioClips;
    #endregion

    public void Awake() {
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Counter/nextNumber")
        };
    }

    public void onButtonPress(entity_player ply) { this.nextNumber(); }
    public void nextNumber() {
        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform);

        this._count = Mathf.Clamp(this._count + 1, 0, 99);
        this.updateText();
    }

    private void updateText() {
        if(this._count < 10) this.text.text = "0" + this._count;
        else this.text.text = this._count.ToString();
    }
}
