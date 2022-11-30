
using TMPro;
using UnityEngine;

public class entity_screen : MonoBehaviour {

    #region PRIVATE
        private TextMeshPro _text;
    #endregion

    public void Awake() {
        this._text = GetComponentInChildren<TextMeshPro>(true);
        CoreController.Instance.OnGamePowerStatusChange += this.onPowerChange;
    }

    private void onPowerChange(GAMEPLAY_POWER_STATUS status) {
        this._text.enabled = status == GAMEPLAY_POWER_STATUS.HAS_POWER;
    }
}
