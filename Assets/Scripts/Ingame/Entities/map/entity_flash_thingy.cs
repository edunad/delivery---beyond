
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class entity_flash_thingy : MonoBehaviour {
    [Header("Settings")]
    public float rechargeTime = 2f;
    public Camera cam;
    public Light cam_light;
    public Material cam_material;

    #region PRIVATE
        private RenderTexture _renderTexture;
        private AudioClip[] _audioClips;
        private bool _canFlash;
    #endregion

    public void Awake() {
        this.cam.enabled = false;
        this.cam_light.enabled = false;

        this._canFlash = true;
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/camera_flash"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/camera_recharge"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/565133__unfa__ui-cancel")
        };

        // Generate renderTexture
        this._renderTexture = new RenderTexture(this.cam.pixelWidth, this.cam.pixelHeight, 24, RenderTextureFormat.ARGB32);
        this._renderTexture.useMipMap = false;
        this._renderTexture.antiAliasing = 1;
    }

    public void onPrimaryUse(entity_player ply) {
        if(ply == null) return;
        if(!this._canFlash) {
            SoundController.Instance.Play3DSound(this._audioClips[2], this.transform, 1, 2, 0.5f);
            return;
        }

        Debug.Log("FLASH");
        this._canFlash = false;

        this.cam_light.enabled = true;
        StartCoroutine(snapshot(() => {
            SoundController.Instance.Play3DSound(this._audioClips[0], this.transform, 1, 2, 0.5f);
            util_timer.simple(0.1f, () => {
                this.cam_light.enabled = false;
            });
        }));

        SoundController.Instance.Play3DSound(this._audioClips[1], this.transform, 1, 2, 0.5f);
        util_timer.simple(this.rechargeTime, () => {
            this._canFlash = true;
        });
    }

    public IEnumerator snapshot(Action onDone) {
        yield return new WaitForEndOfFrame();

        RenderTexture.active = this._renderTexture;

        this.cam.enabled = true;
        this.cam.targetTexture = this._renderTexture;

        this.cam.Render();

        // Create a new Texture2D
        Texture2D result = new Texture2D(this.cam.pixelWidth, this.cam.pixelHeight, TextureFormat.ARGB32, false);
        result.ReadPixels(new Rect(0, 0, this.cam.pixelWidth, this.cam.pixelHeight), 0, 0, false);
        result.Apply();

        this.cam_material.SetTexture("_MainTex", result);

        RenderTexture.active = null;
        this.cam.enabled = false;
        this.cam.targetTexture = null;

        onDone.Invoke();
    }
}
