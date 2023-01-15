
using UnityEngine;

public class tutorial_trash : entity_tutorial_zone {

    [Header("Settings")]
    public entity_item_spot itm_magazine;
    public entity_item_spot itm_box;
    public entity_item_trash itm_trash;

    #region PRIVATE
        private int _trashedItems = 0;
    #endregion

    public override void Awake() {
        base.Awake();

        itm_magazine.OnItemPickup += (entity_item t) => {
            itm_magazine.setLocked(true);
        };

        itm_box.OnItemPickup += (entity_item t) => {
            itm_box.setLocked(true);
        };

        itm_trash.OnItemTrashed += (entity_item t) => {
            this._trashedItems += 1;

            if(this._trashedItems >= 2) {
                util_timer.simple(1f, () => {
                    this.nextArea();
                });
            }
        };
    }


    public override void activateArea() {
        base.activateArea();

        util_timer.simple(1f, () => {
            ConversationController.Instance.queueConversation(new Conversation("tutorial_", "????", "Trash both items", 0.75f, 0.8f));
        });
    }
}
