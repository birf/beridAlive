using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Animator blackScreenFadeout;
    AnimationClip clip;
    AnimationEvent evt;
    void Start()
    {
        foreach(AnimationClip c in blackScreenFadeout.runtimeAnimatorController.animationClips)
        {
            if (c.name == "fadeout")
                clip = c;
        }
        evt.functionName = "ChangeScene";
        evt.time = 1.5f;
        clip.AddEvent(evt);
    }
    public void Play()
    {
        SceneManager.LoadScene("prototype 2");
        // blackScreenFadeout.Play("fadeout");
        // gameObject.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
    void ChangeScene()
    {
    }
}

