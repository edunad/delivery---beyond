
using UnityEngine;

public class tutorial_camera : entity_tutorial_zone {

    [Header("Settings")]
    public entity_flash_thingy cam;
    public entity_item_spot spot;
    public ui_jumpscare spook;

    #region PRIVATE
        private bool _flashed = false;
    #endregion

    public override void Awake() {
        base.Awake();

        cam.setLocked(true);
        spot.OnItemDrop += (entity_item itm) => {
            if(!this._flashed) return;
            spot.setLocked(true);

            util_timer.simple(1f, () => {
                this.nextArea();
            });
        };

        spot.OnItemPickup += (entity_item itm) => {
            this._flashed = false;
            spot.setLocked(true);

            ConversationController.Instance.queueConversation(new Conversation("tutorial_takepick", "????", "Take a picture (Mouse1)", 0.75f, 0.8f));
        };

        cam.OnFLASH += () => {
            if(this._flashed) return;
            this._flashed = true;

            spot.setLocked(false);
            spook.jumpscare(false);

            cam.setLocked(true);
            ConversationController.Instance.queueConversation(new Conversation("tutorial_", "????", "Good. Place back the camera.", 0.75f, 0.8f));
        };

        ConversationController.Instance.OnSingleConversationCompleted += (string id) => {
            if(id != "tutorial_takepick") return;
            cam.setLocked(false);
        };
    }

    public override void activateArea() {
        base.activateArea();
        ConversationController.Instance.queueConversation(new Conversation("tutorial_", "????", "Grab the camera", 0.75f, 0.8f));
    }
}
