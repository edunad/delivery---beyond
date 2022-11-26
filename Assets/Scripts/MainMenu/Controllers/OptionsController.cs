
using TMPro;
using UnityEngine;

public class OptionsController : MonoBehaviour {

    public TextMeshPro volumeMesh;
    public TextMeshPro effectMesh;

    public static float musicVolume = 1f;
    public static float effectsVolume = 1f;

    public void Awake () {
        // Get vars
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        effectsVolume = PlayerPrefs.GetFloat("effectsVolume", 1f);

        this.updateText();
    }

    public void OnDestroy() {
        PlayerPrefs.Save();
    }

    /* *************
     * VOLUME
     ===============*/
    public void setMusicVolume(float vol) {
        musicVolume = Mathf.Clamp(vol, 0f, 1f);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);

        // Update the volume
        CoreController.Instance.SendMessage("updateMusicVolume", SendMessageOptions.DontRequireReceiver);
    }

    public void setEffectsVolume(float vol) {
        effectsVolume = Mathf.Clamp(vol, 0f, 1f);
        PlayerPrefs.SetFloat("effectsVolume", effectsVolume);

        // Update the volume
        CoreController.Instance.SendMessage("updateEffectsVolume", SendMessageOptions.DontRequireReceiver);
    }

    /* *************
    * UI
    ===============*/
    private void updateText() {
        this.volumeMesh.text = Mathf.Round(musicVolume * 100).ToString();
        this.effectMesh.text = Mathf.Round(effectsVolume * 100).ToString();
    }

    public void OnUIClick(string element) {
        if (element == "ui_vol_effects_down") {
            this.setEffectsVolume(effectsVolume - 0.01f);
        }else if(element == "ui_vol_effects_up") {
            this.setEffectsVolume(effectsVolume + 0.01f);
        } else if (element == "ui_vol_music_down") {
            this.setMusicVolume(musicVolume - 0.01f);
        } else if (element == "ui_vol_music_up") {
            this.setMusicVolume(musicVolume + 0.01f);
        }

        this.updateText();
    }
}
