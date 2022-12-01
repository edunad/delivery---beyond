
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class entity_printer : MonoBehaviour {
    [Header("Settings")]
    public GameObject paperTemplate;
    public int printTime = 9;

    #region PRIVATE
        private AudioSource _sound;
        private entity_item_spot _spot;
        private AudioClip[] _audioClips;
        private util_timer _timer;
        private bool _printing;
    #endregion

    public void Awake() {
        this._sound = GetComponent<AudioSource>();
        this._sound.playOnAwake = false;
        this._sound.volume = 0.8f;
        this._sound.maxDistance = 10f;

        this._spot = GetComponentInChildren<entity_item_spot>(true);
        this._spot.setLocked(true);

        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        bool hasPower = status == GAMEPLAY_POWER_STATUS.HAS_POWER;
        if(!this._printing) return;

        if(this._timer != null) this._timer.setPaused(!hasPower);
        if(this._sound.isPlaying && !hasPower) this._sound.Pause();
        else if(!this._sound.isPlaying && hasPower) this._sound.UnPause();
    }

    public void print() {
        if(this._printing) return;

        this._spot.deleteItem();
        this._spot.setLocked(true);

        this._printing = true;

        Vector3 startPos = new Vector3(0.0452f, -0.0435f, -0.095f);
        float endPosY = 0.15f;

        GameObject paperInstance = GameObject.Instantiate(this.paperTemplate, startPos, Quaternion.Euler(this.transform.eulerAngles), this.transform);
        this._sound.Play();

        int p = 0;
        this._timer = util_timer.create(this.printTime, 1f, () => {
            paperInstance.transform.localPosition = new Vector3(startPos.x, startPos.y, p * (endPosY - startPos.y) / this.printTime);
            p++;

            if(p >= this.printTime) {
                this._spot.setLocked(false);
                this._spot.placeItem(paperInstance, true);

                this._printing = false;
                this._timer = null;
            }
        });
    }
}
