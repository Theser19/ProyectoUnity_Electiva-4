using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Cambio_Scene: MonoBehaviour
{
    [SerializeField] private Slider loadbar;
    [SerializeField] private GameObject ladpanel;

    public void Sceneload(int sceneIndex)
    {
        ladpanel.SetActive(true);
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            loadbar.value = progress;
            yield return null;
        }
    }
}
