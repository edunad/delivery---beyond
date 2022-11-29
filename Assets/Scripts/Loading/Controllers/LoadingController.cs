using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {
    private int _loadIndex;
    private AsyncOperation _asyncOp;

	public void Start () {
        this._loadIndex = PlayerPrefs.GetInt("loading_scene_index", -1); // There is probably a better way ¯\_(ツ)_/¯
        if (this._loadIndex == -1) throw new UnityException("Invalid scene index");

        StartCoroutine(this.startLoading());
	}

    public void FixedUpdate() {
        util_timer.fixedUpdate();
    }

    private void OnDestroy() {
        util_timer.clear();
    }

    private IEnumerator startLoading() {
        this._asyncOp = SceneManager.LoadSceneAsync(this._loadIndex, LoadSceneMode.Single);
        this._asyncOp.allowSceneActivation = false;

        while (!this._asyncOp.isDone) {
            if (this._asyncOp.progress == 0.9f) {
                util_timer.simple(1f, () => { // Small delay
                    this._asyncOp.allowSceneActivation = true;
                    PlayerPrefs.SetInt("loading_scene_index", -1); // Reset
                });
            }

            yield return null;
        }
    }
}
