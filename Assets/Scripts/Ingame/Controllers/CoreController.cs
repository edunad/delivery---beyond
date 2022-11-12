
using UnityEngine;

public class CoreController : MonoBehaviour {
    public void Update() {
        util_timer.Update();
    }

    public void OnDestroy() {
        util_timer.Clear();
    }
}
