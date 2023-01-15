
using UnityEngine;

[RequireComponent(typeof(entity_teleport))]
public class entity_tutorial_zone : MonoBehaviour {
    public Transform teleportZone;

    [HideInInspector]
    public entity_light spotlight;

    #region PRIVATE
        private entity_teleport _teleport;
    #endregion

    public virtual void Awake() {
        this._teleport = GetComponent<entity_teleport>();
        this.spotlight = GetComponentInChildren<entity_light>(true);
    }

    public virtual void activateArea() {
        this.spotlight?.on(false);
    }

    public void nextArea() {
        this.spotlight?.off(false);

        util_timer.simple(0.5f, () => {
            entity_tutorial_zone nextArea = TutorialController.Instance.activateNextArea();
            if(nextArea == null) return;

            this._teleport.teleport(nextArea.gameObject.transform);
        });
    }
}
