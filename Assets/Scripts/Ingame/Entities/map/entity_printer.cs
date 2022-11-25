
using UnityEngine;

public class entity_printer : MonoBehaviour {
    [Header("Settings")]
    public GameObject paperTemplate;
    public int printTime = 9;

    #region PRIVATE
        private entity_item_spot _spot;
        private AudioClip[] _audioClips;
        private bool _printing;
    #endregion

    public void Awake() {
        this._spot = GetComponentInChildren<entity_item_spot>(true);
        this._spot.setLocked(true);

        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Printer/181420__viertelnachvier__old-dot-matrix-printer"),
        };
    }

    public void print() {
        if(this._printing) return;

        this._spot.deleteItem();
        this._spot.setLocked(true);

        this._printing = true;

        Vector3 startPos = new Vector3(0.0452f, -0.0435f, -0.095f);
        float endPosY = 0.15f;

        GameObject paperInstance = GameObject.Instantiate(this.paperTemplate, startPos, Quaternion.Euler(this.transform.eulerAngles), this.transform);
        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform);

        int p = 0;
        util_timer.create(this.printTime, 1f, () => {
            paperInstance.transform.localPosition = new Vector3(startPos.x, startPos.y, p * (endPosY - startPos.y) / this.printTime);
            p++;

            if(p >= this.printTime) {
                this._spot.setLocked(false);
                this._spot.placeItem(paperInstance, true);

                this._printing = false;
                Debug.Log("Done printing");
            }
        });
    }
}
