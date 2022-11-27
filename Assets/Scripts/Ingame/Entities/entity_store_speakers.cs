
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

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
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
