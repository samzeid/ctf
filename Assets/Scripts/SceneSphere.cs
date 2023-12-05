using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneSphere : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler
{
    public string sceneName;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SFX.Play(0);
        GetComponent<Animator>().SetBool("selected", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Animator>().SetBool("selected", false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SFX.Play(1);
        if (sceneName != null)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Animator>().SetBool("clicked", false);
    }
}
