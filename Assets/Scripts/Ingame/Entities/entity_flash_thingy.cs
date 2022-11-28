
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class entity_flash_thingy : MonoBehaviour {
    [Header("Settings")]
    public float rechargeTime = 2f;
    public Material cam_material;

    #region PRIVATE
        private Camera _cam;
        private Light _cam_light;
        private RenderTexture _renderTexture;
        private AudioClip[] _audioClips;
        private bool _canFlash;
    #endregion

    public void Awake() {
        this._cam = GetComponentInChildren<Camera>(true);
        this._cam.enabled = false;

        this._cam_light = GetComponentInChildren<Light>(true);
        this._cam_light.enabled = false;

        this._canFlash = true;
        this._audioClips = new AudioClip[] {
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/camera_flash"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/camera_recharge"),
            AssetsController.GetResource<AudioClip>("Sounds/Ingame/Objects/Flash/565133__unfa__ui-cancel")
        };

        // Generate renderTexture
        this._renderTexture = new RenderTexture(this._cam.pixelWidth, this._cam.pixelHeight, 24, RenderTextureFormat.ARGB32);
        this._renderTexture.useMipMap = false;
        this._renderTexture.antiAliasing = 1;

        this.cam_material.SetColor("_Color", Color.black);
    }

    public void onPrimaryUse(entity_player ply) {
        if(ply == null) return;
        if(!this._canFlash) {
            SoundController.Instance.Play3DSound(this._audioClips[2], this.transform, 1, 2, 0.5f);
            return;
        }

        this._canFlash = false;
        this._cam_light.enabled = true;

        StartCoroutine(snapshot(() => {
            SoundController.Instance.Play3DSound(this._audioClips[0], this.transform, 1, 2, 0.5f);
            util_timer.simple(0.1f, () => {
                this._cam_light.enabled = false;
            });
        }));

        SoundController.Instance.Play3DSound(this._audioClips[1], this.transform, 1, 2, 0.5f);
        util_timer.simple(this.rechargeTime, () => {
            this._canFlash = true;
        });
    }

    public IEnumerator snapshot(Action onDone) {

        yield return new WaitForEndOfFrame();

        RenderSettings.fog = false;
        RenderTexture.active = this._renderTexture;

        this._cam.enabled = true;
        this._cam.targetTexture = this._renderTexture;

        this._cam.Render(); // Force a render
        this._cam.Render(); // ???

        // Create a new Texture2D
        Texture2D result = new Texture2D(this._cam.pixelWidth, this._cam.pixelHeight, TextureFormat.ARGB32, false);
        result.ReadPixels(new Rect(0, 0, this._cam.pixelWidth, this._cam.pixelHeight), 0, 0, false);
        result.Apply();

        this.cam_material.SetColor("_Color", Color.white);
        this.cam_material.SetTexture("_MainTex", result);

        RenderTexture.active = null;
        RenderSettings.fog = true;
        this._cam.enabled = false;
        this._cam.targetTexture = null;

        onDone.Invoke();
    }
}
