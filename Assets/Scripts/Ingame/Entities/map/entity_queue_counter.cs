
using TMPro;
using UnityEngine;

public class entity_queue_counter : MonoBehaviour {
    [Header("Settings")]
    public TextMeshPro text;

    #region PRIVATE
        private AudioClip[] _audioClips;
    #endregion

    public void Awake() {
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Counter/255156__jmayoff__bell-ding")
        };

        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        this.text.enabled = status == GAMEPLAY_POWER_STATUS.HAS_POWER;
    }

    private void updateText(int count) {
        if(count < 10) this.text.text = "0" + count;
        else this.text.text = count.ToString();
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        if(newStatus != GAMEPLAY_STATUS.SERVING) return;

        SoundController.Instance.Play3DSound(this._audioClips[0], this.transform);
        this.updateText(CoreController.Instance.totalClients);
    }
}
