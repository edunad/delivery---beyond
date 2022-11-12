
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class entity_player : MonoBehaviour {

	[Header("Player")]
	public float moveSpeed = 10f;
	public float maxGrabDistance = 1f;

	public LayerMask usableMask = 1 << 6;
	public GameObject holdSpot;

	[Header("Camera")]
	public float sensitivity = 10f;

	[Header("Other")]
	public entity_item holdingItem; // TODO: Support multiple items?


    private CharacterController _controller;
    private Camera _camera;
	private float _camRotationY;

    // Use this for initialization
	public void Awake () {
		this._controller = GetComponent<CharacterController>();
		this._camera = GetComponentInChildren<Camera>();

		// Hide cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		this.name = "entity_player";
	}

	public void Update() {
		if(this._controller != null) {
			this._controller.Move((transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal")).normalized * moveSpeed * Time.deltaTime);
			if (!this._controller.isGrounded) this._controller.Move(Vector3.up * Time.deltaTime * Physics.gravity.y);
		}

		if(this._camera != null) {
			// Camera movement
			_camRotationY += Input.GetAxis("Mouse Y") * sensitivity;
			_camRotationY = Mathf.Clamp(_camRotationY, -90f, 90f);

			this._camera.transform.localRotation = Quaternion.Euler(-_camRotationY, 0f, 0f);
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivity, 0);
			// --------------

			// INPUTS
			RaycastHit hit;
			if (Physics.Raycast(this._camera.ScreenPointToRay(Input.mousePosition), out hit, maxGrabDistance, usableMask)) {
				if(Input.GetKeyDown(KeyCode.E)) this.onUse(hit.collider.gameObject); // USE
				// TODO: Draw highlight on shader
			}

			if(this.holdingItem != null){
				if(Input.GetMouseButton(0)) {
					this.holdingItem.gameObject.BroadcastMessage("onPrimaryUse", this, SendMessageOptions.DontRequireReceiver);
				} else if(Input.GetMouseButton(1)) {
					this.holdingItem.gameObject.BroadcastMessage("onSecondaryUse", this, SendMessageOptions.DontRequireReceiver);
				}
			}
			// --------------
		}
	}

	public bool hasItem() {
		return this.holdingItem != null;
	}

	private void holdObject(entity_item pick) {
		if(pick == null || this.hasItem()) return;

		this.holdingItem = pick;
		this.holdingItem.setOwner(this.gameObject, this.holdSpot.transform.localRotation, this.holdSpot.transform);
	}

	public void onUse(GameObject obj) {
		if(obj == null) return;

		if(obj.name.StartsWith("place-")) {
			entity_placeable place = obj.GetComponent<entity_placeable>();
			if(place == null) throw new System.Exception("Invalid entity_placeable");

			if(place.hasItem()) {
				if(this.hasItem()) return;
				this.holdObject(place.takeItem());
			} else {
				if(!this.hasItem()) return;
				if(place.placeItem(this.holdingItem)) this.holdingItem = null;
			}
		}
	}

	/* *************
     * DEBUG
     ===============*/
    public void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
		//Gizmos.DrawLine(this.transform.position, this.transform.forward * this.maxGrabDistance);
        Gizmos.color = Color.white;
    }
}
