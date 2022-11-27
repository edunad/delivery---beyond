
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-1)]
public class OptionsController : MonoBehaviour {
    public static OptionsController Instance { get; private set; }

    [Header("Settings")]
    public TextMeshPro volumeMesh;
    public TextMeshPro effectMesh;
    public TextMeshPro sensitivityMesh;

    [Header("Options")]
    public GameObject optionsMenu;

    [HideInInspector]
    public float musicVolume = 1f;
    [HideInInspector]
    public float effectsVolume = 1f;
    [HideInInspector]
    public int sensitivity = 10;

    public delegate void onSettingsUpdated(string id, object val);
    public event onSettingsUpdated OnSettingsUpdated;

    public delegate void onOptionsMenuUpdate(bool open);
    public event onOptionsMenuUpdate OnOptionsMenuUpdate;

    public void Awake () {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;

        // Get vars
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        effectsVolume = PlayerPrefs.GetFloat("effectsVolume", 1f);
        sensitivity = PlayerPrefs.GetInt("sensitivity", 3);

        this.updateText();
    }

    public void OnDestroy() {
        PlayerPrefs.Save();
    }

    public void displayOptions(bool set) {
        this.optionsMenu.SetActive(set);
        if(OnOptionsMenuUpdate != null) OnOptionsMenuUpdate.Invoke(set);
    }
    public bool isOptionsOpen() { return this.optionsMenu.activeInHierarchy; }

    public void Update() {
        if(CoreController.Instance == null) return; // Not in ingame

        if(!Input.GetKeyDown(KeyCode.Escape)) return;
        this.displayOptions(!this.isOptionsOpen());
    }

    /* *************
     * VOLUME
     ===============*/
    public void setMusicVolume(float vol) {
        musicVolume = Mathf.Clamp(vol, 0f, 1f);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);

        if(OnSettingsUpdated != null) OnSettingsUpdated.Invoke("musicVolume", musicVolume);
    }

    public void setSensitivity(int sen) {
        sensitivity = Mathf.Clamp(sen, 1, 10);
        PlayerPrefs.SetInt("sensitivity", sensitivity);

        if(OnSettingsUpdated != null) OnSettingsUpdated.Invoke("sensitivity", sen);
    }

    public void setEffectsVolume(float vol) {
        effectsVolume = Mathf.Clamp(vol, 0f, 1f);
        PlayerPrefs.SetFloat("effectsVolume", effectsVolume);

        if(OnSettingsUpdated != null) OnSettingsUpdated.Invoke("effectsVolume", effectsVolume);
    }

    /* *************
    * UI
    ===============*/
    private void updateText() {
        this.volumeMesh.text = Mathf.Round(musicVolume * 100).ToString();
        this.effectMesh.text = Mathf.Round(effectsVolume * 100).ToString();
        this.sensitivityMesh.text = sensitivity.ToString();
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
        } else if (element == "ui_sensitivity_down") {
            this.setSensitivity(sensitivity - 1);
        } else if (element == "ui_sensitivity_up") {
            this.setSensitivity(sensitivity + 1);
        } else if (element == "ui_options_close") {
            this.displayOptions(false);
            return;
        } else if (element == "ui_options_backmenu") {
            SceneManager.LoadScene("mainmenu", LoadSceneMode.Single);
            return;
        }

        this.updateText();
    }
}
