
using TMPro;
using UnityEngine;

public class entity_floppy_board : MonoBehaviour {
    [Header("Settings")]
    public TextMeshPro content;

    public void Awake() {
        CoreController.Instance.OnGameStatusUpdated += this.gameStatusChange;
    }

    private void gameStatusChange(GAMEPLAY_STATUS prevStatus, GAMEPLAY_STATUS newStatus) {
        if(prevStatus != GAMEPLAY_STATUS.PREPARING || newStatus != GAMEPLAY_STATUS.IDLE) return;
        string txt = "";

        var codes = CoreController.Instance.floppyCodes;
        foreach(var code in codes) {
            txt += "<align=\"right\"><color=#"+ColorUtility.ToHtmlStringRGB(code.Value.Item2)+">\n";
            txt += "------------ " + code.Key.ToString();
            txt += "</color>\n";
            txt += "<align=\"left\">";
            foreach(GAME_COUNTRIES country in code.Value.Item1) {
                txt += country.ToString().Replace("_", " ") + "\n";
            }
        }

        this.content.text = txt;
    }
}
