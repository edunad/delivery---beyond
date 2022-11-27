
using UnityEngine;

[DisallowMultipleComponent]
public class UIController : MonoBehaviour {
    public static UIController Instance { get; private set; }

    [Header("Settings")]
    public ui_jumpscare spook;

    #region PRIVATE
    #endregion

    public void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }else Instance = this;
    }
}
