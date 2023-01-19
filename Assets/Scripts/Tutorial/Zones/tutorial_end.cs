
using UnityEngine;

public class tutorial_end : entity_tutorial_zone {

    public override void Awake() {
        base.Awake();

        ConversationController.Instance.clear();
        ConversationController.Instance.OnSingleConversationCompleted += (string id) => {
            if(id != "tutorial_end") return;

            util_timer.simple(1f, () => {
                this.nextArea();
            });
        };
    }

    public override void activateArea() {
        base.activateArea();

        util_timer.simple(1f, () => {
            ConversationController.Instance.queueConversation(new Conversation("tutorial_end", "????", "Do. Not. Disappoint us.", 0.75f, 0.8f));
        });
    }

}
