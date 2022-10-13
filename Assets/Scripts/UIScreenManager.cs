using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreenManager : MonoBehaviour
{
    [SerializeField] GameObject AnimationBackground;
    [SerializeField] GameObject AnimationBackgroundBack;
    [SerializeField] GameObject MainMenuCanvas;
    [SerializeField] GameObject MultiplayerCanvas;

    public void OpenMultiplayerTab()
    {
        StartCoroutine(OpenMultiplayer());   
    }

    public void OpenMainMenuTab()
    {
        StartCoroutine(OpenMainMenu());
    }

    IEnumerator OpenMultiplayer()
    {
        AnimationBackground.SetActive(false);
        AnimationBackgroundBack.SetActive(true);
        yield return new WaitForSeconds(2f);
        MainMenuCanvas.SetActive(false);
        MultiplayerCanvas.SetActive(true);
        AnimationBackgroundBack.SetActive(false);
        AnimationBackground.SetActive(true);
        yield return new WaitForSeconds(2f);
        AnimationBackground.SetActive(false);
    }

    IEnumerator OpenMainMenu()
    {
        AnimationBackground.SetActive(false);
        AnimationBackgroundBack.SetActive(true);
        yield return new WaitForSeconds(2f);
        MultiplayerCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
        AnimationBackgroundBack.SetActive(false);
        AnimationBackground.SetActive(true);
        yield return new WaitForSeconds(2f);
        AnimationBackground.SetActive(false);
    }
}
