using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class TooltipCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject ui;

    public GameObject selectionUI;

    private bool showing = false;

    public static List<TooltipCollider> tooltips = new List<TooltipCollider>();

    public static GameObject ModalOpen = null;

    private float openTime = 0;
    private bool hovering = false;

    [HideInInspector]
    public InteractiveObject subCollider;

    public static bool IsSelectedModal(GameObject obj)
    {
        return ModalOpen == obj || (ModalOpen != null && obj == ModalOpen.GetComponent<TooltipCollider>().subCollider.gameObject);
    }

    public void OnDestroy()
    {
        tooltips.Remove(this);
    }

    void Start()
    {
        //RectTransform rt = selectionUI.GetComponent<RectTransform>();
        //print(selectionUI.transform.localPosition + "   " + rt.sizeDelta);

        tooltips.Add(this);

        DebugOutput.Log("-");

        subCollider = GetComponentInChildren<InteractiveObject>();
        if (subCollider)
        {
            subCollider.onClick.AddListener(OnPointerClick);
            subCollider.onPointerEnter.AddListener(OnPointerEnter);
            subCollider.onPointerExit.AddListener(OnPointerExit);
            subCollider.onPointerDown.AddListener(OnPointerDown);
            subCollider.onPointerUp.AddListener(OnPointerUp);
        }
    }

    public static void HideOthers(GameObject mainObject)
    {
        ModalOpen = mainObject;
        foreach (TooltipCollider t in tooltips)
        {
            if (t.gameObject != mainObject)
                t.ModalHide();
        }
    }

    public static void ShowAll()
    {
        ModalOpen = null;
        foreach (TooltipCollider t in tooltips)
        {
            t.ModalShow();
        }
    }

    void Update()
    {
        if (ModalOpen == gameObject)
        {
            //DebugOutput.Log("ModalOpen: " + (Time.time - openTime) + "  " + Vector3.Angle(transform.forward, Camera.main.transform.forward) + "  " + hovering);
            if (Time.time - openTime > 2.5f && !hovering
                && Vector3.Angle(transform.forward, Camera.main.transform.forward) > 41.0f
                )
            {
                //DebugOutput.Log("AutoClose");
                ShowAll();
                HideUI();
                showing = false;
            }
        }
    }

    public void ModalHide()
    {
        if (ui.GetComponent<Animator>())
            ui.GetComponent<Animator>().SetBool("hidden", true);

        if (selectionUI)
            selectionUI.SetActive(false);
    }

    public void ModalShow()
    {
        if (ui.GetComponent<Animator>())
            ui.GetComponent<Animator>().SetBool("hidden", false);

    }

    public void OnPointerEnter()
    {
        OnPointerEnter(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        ui.SetActive(true);
        SFX.Play(0);
        if (ui.GetComponent<Animator>())
            ui.GetComponent<Animator>().SetBool("hover", true);
        //ui.GetComponent<Animator>().SetTrigger("show");
    }

    public void OnPointerExit()
    {
        OnPointerExit(null);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        if (ui.GetComponent<Animator>())
            ui.GetComponent<Animator>().SetBool("hover", false);
        //ui.GetComponent<Animator>().SetTrigger("hide");
    }

    public void OnPointerClick()
    {
        OnPointerClick(null);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DoClick();
    }

    private void DoClick()
    {
        showing = !showing;
        
        if (showing)
        {
            openTime = Time.time;
            HideOthers(gameObject);
            ui.SetActive(true);
            SFX.Play(1);
            if (ui.GetComponent<Animator>())
            {
                ui.GetComponent<Animator>().SetBool("show", true);
                ui.GetComponent<Animator>().SetBool("faded", true);
            }

            if (selectionUI)
                selectionUI.SetActive(true);
        }
        else
        {
            ShowAll();
            HideUI();

        }

        //Invoke("HideUI", 3.0f);
    }

    public void HideUI()
    {
        if (ui.GetComponent<Animator>())
            ui.GetComponent<Animator>().SetBool("show", false);

        if (selectionUI)
            selectionUI.SetActive(false);
    }


    public void OnPointerDown()
    {
        OnPointerDown(null);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //OnPointerClick();
    }

    public void OnPointerUp()
    {
        OnPointerUp(null);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
}
