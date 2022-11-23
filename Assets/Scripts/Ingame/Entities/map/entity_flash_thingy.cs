
using System.Linq;
using UnityEngine;

public class entity_flash_thingy : MonoBehaviour {
    [Header("Settings")]
    public float rechargeTime = 2f;

    #region PRIVATE
        private AudioClip[] _audioClips;
        private bool _canFlash;
    #endregion

    public void Awake() {
        this._canFlash = true;
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/camera_flash"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/camera_recharge"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/565133__unfa__ui-cancel")
        };
    }

    public void onPrimaryUse(entity_player ply) {
        if(ply == null) return;
        if(!this._canFlash) {
            SoundController.Instance.Play3DSound(this._audioClips[2], this.transform, 1, 2, 0.5f);
            return;
        }

        Debug.Log("FLASH");
        this._canFlash = false;

        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform, 1, 2, 0.5f);
        SoundController.Instance.Play3DSound(this._audioClips[1], this.transform, 1, 2, 0.5f);
        util_timer.simple(this.rechargeTime, () => {
            this._canFlash = true;
        });
    }
}
