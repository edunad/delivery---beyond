
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class entity_store_music : MonoBehaviour {

    [Header("Settings")]
    public List<AudioClip> storeMusic = new List<AudioClip>();
    public AudioClip storeAnnouncement;
    public float volume;

    #region PRIVATE
        private entity_store_speakers[] _speakers;
        private util_timer _timer;
    #endregion

    public void Awake() {
        this._speakers = GetComponentsInChildren<entity_store_speakers>(true);
        this.pickSong();

        if(CoreController.Instance != null) {
            CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
            CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
        }
    }

    private void pickSong() {
        if(this._timer != null) this._timer.stop();
        if(CoreController.Instance != null && CoreController.Instance.status == GAMEPLAY_STATUS.WIN) return;

        Random.InitState(System.DateTime.Now.Millisecond); // Re-randomize seed
        AudioClip clip = this.storeMusic[Random.Range(0, this.storeMusic.Count)];

        foreach(entity_store_speakers speaker in this._speakers) {
            speaker.stop();
            speaker.setClip(clip);
            speaker.setVolume(this.volume);
            speaker.play();
        }

        this._timer = util_timer.simple(clip.length, () => {
            this.pickSong();
        });
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        if(this._timer != null) this._timer.setPaused(status == GAMEPLAY_POWER_STATUS.NO_POWER);
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        if(newStatus != GAMEPLAY_STATUS.WIN) return;
        if(this._timer != null) this._timer.stop();

        foreach(entity_store_speakers speaker in this._speakers) {
            speaker.stop();
            speaker.setClip(this.storeAnnouncement);
            speaker.setVolume(1f);
            speaker.play();
        }
    }
}
