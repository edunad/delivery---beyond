
using UnityEngine;

public class tutorial_pickup : entity_tutorial_zone {

    [Header("Settings")]
    public entity_item_spot itm_start;
    public entity_item_spot itm_end;

    public override void Awake() {
        base.Awake();

        itm_start.OnItemPickup += (entity_item t) => {
            itm_start.setLocked(true);
        };

        itm_end.OnItemDrop += (entity_item t) => {
            itm_end.setLocked(true);

            util_timer.simple(1f, () => {
                this.nextArea();
            });
        };
    }

    public override void activateArea() {
        base.activateArea();

        util_timer.simple(1f, () => {
            ConversationController.Instance.queueConversation(new Conversation("tutorial_", "????", "Pickup the box (E) and place it on the other slot (E)", 0.75f, 0.8f));
        });
    }
}
