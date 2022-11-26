
using UnityEngine;

public class ui_button : MonoBehaviour {

    public string id;
    public GameObject target;
    public Camera uiCamera;
    public Rect clickArea;

    public void Update() {
        if(!Input.GetMouseButtonDown(0)) return;

        if(this.target == null) return;
        if(this.uiCamera == null || !this.uiCamera.enabled) return;

        Vector2 pos = Input.mousePosition;
        Vector3 uiStart = this.transform.position + new Vector3(clickArea.x, clickArea.y, 0f);

        Vector2 startPos = this.uiCamera.WorldToScreenPoint(uiStart);
        Vector2 endPos = this.uiCamera.WorldToScreenPoint(uiStart + new Vector3(clickArea.width, clickArea.height, 0f));

        if (pos.x >= startPos.x && pos.x <= endPos.x && pos.y >= startPos.y && pos.y <= endPos.y) {
            this.target.SendMessage("OnUIClick", this.id, SendMessageOptions.RequireReceiver);
        }
    }

    public void OnDrawGizmos() {
        Vector3 col = this.transform.position + new Vector3(clickArea.x, clickArea.y, 0f);

        float offsetX = col.x + clickArea.width / 2;
        float offsetY = col.y + clickArea.height / 2;

        // Draw duck area
        Gizmos.color = new Color(255, 0, 255, 100);
        Gizmos.DrawWireCube(new Vector3(offsetX, offsetY, col.z), new Vector3(clickArea.width, clickArea.height, col.z + 0.5f));
    }
}
