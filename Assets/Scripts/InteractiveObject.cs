using UnityEngine;
using UnityEngine.EventSystems;

public class InteractiveObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public UnityEngine.Events.UnityEvent onClick = null;

    public UnityEngine.Events.UnityEvent onPointerEnter = null;
    public UnityEngine.Events.UnityEvent onPointerExit = null;

    public UnityEngine.Events.UnityEvent onPointerDown = null;
    public UnityEngine.Events.UnityEvent onPointerUp = null;

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        SFX.Play(0);
        if (GetComponent<Animator>())
            GetComponent<Animator>().SetBool("selected", true);
        onPointerEnter.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (GetComponent<Animator>())
            GetComponent<Animator>().SetBool("selected", false);
        onPointerExit.Invoke();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        SFX.Play(1);
        onClick.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Animator>())
            GetComponent<Animator>().SetBool("clicked", true);
        onPointerDown.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GetComponent<Animator>())
            GetComponent<Animator>().SetBool("clicked", false);
        onPointerUp.Invoke();
    }
}