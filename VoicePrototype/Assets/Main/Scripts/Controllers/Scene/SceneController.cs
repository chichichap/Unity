using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    private AsyncOperation _asyncOperation;

    [SerializeField]
    private float _loadDelay = 1f;

    public void DoFadeIn() {
        if (PlayerController.main != null) {
            PlayerController.main.OvrScreenFade.DoFadeIn();
        }
    }

    public void DoFadeOut() {
        if (PlayerController.main != null)
        {
            PlayerController.main.OvrScreenFade.DoFadeOut();
        }
    }

    public void DoLoadScene(string sceneName) {
        StartCoroutine(DelayedLoadScene(sceneName));
    }

    public void DoStartLoadingScene(string sceneName) {
        StartCoroutine(StartLoadingScene(sceneName));
    }

    public void DoFinishLoadingScene() {
        if (_asyncOperation != null)
            _asyncOperation.allowSceneActivation = true;
    }

    IEnumerator DelayedLoadScene(string sceneName) {
        yield return new WaitForSecondsRealtime(_loadDelay);

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator StartLoadingScene(string sceneName)
    {
        _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        _asyncOperation.allowSceneActivation = false;

        while (!_asyncOperation.isDone)
        {
            if (_asyncOperation.progress >= 0.9f)
            {
                //ready to finish loading
            }

            yield return null;
        }
    }
}
