using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroVideoSceneProgressor : MonoBehaviour
{
    [SerializeField] private float m_waitTime;

    private void Start()
    {
        StartCoroutine(ProgressRoutine());
    }

    private IEnumerator ProgressRoutine()
    {
        yield return new WaitForSeconds(m_waitTime);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
