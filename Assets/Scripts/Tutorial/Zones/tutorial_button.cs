
using UnityEngine;

public class tutorial_button : entity_tutorial_zone {

    [Header("Settings")]
    public entity_button btn;

    public override void Awake() {
        base.Awake();

        btn.setButtonLocked(true);
        btn.OnUSE += (entity_player ply) => {
            util_timer.simple(1f, () => {
                this.nextArea();
            });
        };

        ConversationController.Instance.OnSingleConversationCompleted += (string id) => {
            if(id != "tutorial_btn") return;
            btn.setButtonLocked(false);
        };
    }

    public override void activateArea() {
        base.activateArea();

        util_timer.simple(1f, () => {
            ConversationController.Instance.queueConversation(new Conversation("tutorial_", "????", "You are finally awake..", 0.75f, 0.8f));
            ConversationController.Instance.queueConversation(new Conversation("tutorial_btn", "????", "Press the button (E)", 0.75f, 0.8f));
        });
    }
}
