
using System.Linq;
using TMPro;
using UnityEngine;

public class entity_floppy_container : MonoBehaviour {
    public void Awake() {
        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        if(prevStatus != GAMEPLAY_STATUS.PREPARING || newStatus != GAMEPLAY_STATUS.IDLE) return;

        entity_floppy[] floppies = GetComponentsInChildren<entity_floppy>();
        var codes = CoreController.Instance.floppyCodes;

        for(int i = 0; i < codes.Count; i++) {
            GAME_REGIONS region = codes.Keys.ToList()[i];
            floppies[i].setRegion(region, codes[region].Item2);
        }
    }
}
