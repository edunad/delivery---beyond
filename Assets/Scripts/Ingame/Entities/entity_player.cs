
using UnityEngine;

public enum ShakeMode {
    SHAKE_UP = 1,
    SHAKE_DOWN = 2,
    SHAKE_LEFT = 3,
    SHAKE_RIGHT = 4,
    SHAKE_ALL = 5
}

[RequireComponent(typeof(CharacterController))]
public class entity_player : MonoBehaviour {

	[Header("Player")]
	public float moveSpeed = 10f;
	public float maxGrabDistance = 1f;

	public LayerMask usableMask = 1 << 6;
	public Transform big_item_position;
	public Transform small_item_position;

	[Header("Camera")]
	public float sensitivity = 10f;

	[HideInInspector]
	public entity_item holdingItem; // TODO: Support multiple items?

	// PRIVATE ---
	#region PRIVATE
		#region SHAKE
			private bool _isCameraShaking;
			private ShakeMode _shakemode;
			private float _shakePower;
		#endregion

		#region OBJS
			private CharacterController _controller;
			private Camera _camera;
		#endregion

		#region CAMERA
			private float _camRotationY;
		#endregion

		#region OTHER
			private bool _frozen;
		#endregion
	#endregion


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
		if(this._frozen) return;

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

			if (this._isCameraShaking) {
       			Vector3 final = this._camera.transform.localPosition;

				switch (_shakemode) {
					case ShakeMode.SHAKE_ALL:
						final.z += Random.value * this._shakePower * 2 - this._shakePower;
						final.x += Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_UP:
						final.z += Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_DOWN:
						final.z -= Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_LEFT:
						final.x += Random.value * this._shakePower * 2 - this._shakePower;
					break;
					case ShakeMode.SHAKE_RIGHT:
						final.x -= Random.value * this._shakePower * 2 - this._shakePower;
					break;

					default:
					break;
				}

       			this._camera.transform.localPosition = final;
			}
		}
	}

	public void shakeCamera(float time, float power = 0.005f, ShakeMode shakemode = ShakeMode.SHAKE_ALL) {
		this._isCameraShaking = true;
		this._shakePower = power;
		this._shakemode = shakemode;

		util_timer.Simple(time, () => {
			this._isCameraShaking = false;
			this._camera.transform.localPosition = Vector3.zero;
		});
	}

	public void freeze(bool set) {
		this._frozen = set;
	}

	public bool isHoldingItem() {
		return this.holdingItem != null;
	}

	private void holdObject(entity_item pick) {
		if(pick == null || this.isHoldingItem()) return;

		this.holdingItem = pick;

		Transform pos = small_item_position;
		if(pick.isBig) pos = big_item_position;

		this.holdingItem.setOwner(this.gameObject, pos);
	}

	public void onUse(GameObject obj) {
		if(obj == null) return;

		if(obj.name.StartsWith("itm-spot-")) {
			entity_item_spot spot = obj.GetComponent<entity_item_spot>();
			if(spot == null) throw new System.Exception("Invalid entity_item_spot");

			if(spot.hasItem()) {
				if(this.isHoldingItem()) return;
				this.holdObject(spot.takeItem());
			} else {
				if(!this.isHoldingItem()) return;
				if(spot.placeItem(this.holdingItem)) this.holdingItem = null;
			}
		} else if(obj.name.StartsWith("itm-spawner-")) {
			if(this.isHoldingItem()) return;

			entity_item_spawner spawner = obj.GetComponent<entity_item_spawner>();
			if(spawner == null) throw new System.Exception("Invalid entity_item_spawner");
			if(!spawner.canTakeItem(this.gameObject)) return;

			if(spawner.canSpawnItem()) this.holdObject(spawner.takeItem(this.gameObject));
		} else if(obj.name.StartsWith("itm-trash-")) {
			if(!this.isHoldingItem()) return;

			entity_item_trash trash = obj.GetComponent<entity_item_trash>();
			if(trash == null) throw new System.Exception("Invalid entity_item_trash");

			if(trash.canTrash(this.holdingItem.id)) trash.trashItem(this.holdingItem);
		} else {
			obj.BroadcastMessage("onPlayerUse", this, SendMessageOptions.RequireReceiver);
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
