using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;

[System.Serializable]
public class StringContainer
{
    public string str;
}

public class Interactable : MonoBehaviour
{
    public bool isInteractive = true;
    public bool preventDefault = false;
    public bool canBeDescribed = false;
    
    [SerializeField]
    UnityEvent e;

    [SerializeField]
    UnityEvent mouseEnterCustomEvent;
    [SerializeField]
    UnityEvent mouseExitCustomEvent;

    [HideInInspector]
    public UnityEvent<StringContainer> getDescriptionEvent;

    Transform t;

    [HideInInspector]
    public Vector3 initialScale;


    private Sequence lastTween = null;


    void Awake()
    {
        t = transform;
        initialScale = t.localScale;

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void PerformCancelAction()
    {
        t.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(t.DORotate(new Vector3(0, 0, 5), 0.04f));
        seq.Append(t.DORotate(new Vector3(0, 0, -5), 0.08f));
        seq.Append(t.DORotate(new Vector3(0, 0, 0), 0.04f));
        seq.SetLoops(2);
        seq.OnKill(() => { t.rotation = Quaternion.identity; });
    }


    public void OnMouseEnter()
    {
        if(canBeDescribed)
        {
            G.describerT.gameObject.SetActive(true);
            G.describerT.position = (Vector3)G.mousePosition + Vector3.up * 1.5f;
            StringContainer strCont = new StringContainer();
            getDescriptionEvent?.Invoke(strCont);
            G.describerTe.text = strCont.str;
        }
        if (lastTween != null)
        {
            lastTween.Kill();
        }
        if (!preventDefault)
        {
            lastTween = DOTween.Sequence();
            lastTween.Append(t.DOScale(1.03f * initialScale, 0.2f));
        }
        mouseEnterCustomEvent.Invoke();
    }
    private void OnMouseOver()
    {
        if (canBeDescribed)
        {
            G.describerT.position = (Vector3)MaximUtils.ClampToScreen((Vector3)G.mousePosition + Vector3.up * 1.5f, new Vector2(3.6f, 1.8f));
        }
    }

    public void OnMouseExit()
    {
        if (canBeDescribed)
        {
            G.describerT.gameObject.SetActive(false);
        }
        if (lastTween != null)
        {
            lastTween.Kill();
        }
        if (!preventDefault)
        {
            lastTween = DOTween.Sequence();
            lastTween.Append(t.DOScale(1f * initialScale, 0.2f));
        }

        mouseExitCustomEvent.Invoke();
    }


    private void OnMouseDown()
    {
        if (isInteractive)
        {
            //Animation
            if (lastTween != null)
            {
                lastTween.Kill();
            }
            if (!preventDefault)
            {
                lastTween = DOTween.Sequence();
                lastTween.Append(t.DOScale(0.95f * initialScale, 0.1f));
                lastTween.Append(t.DOScale(1.03f * initialScale, 0.1f));
            }
            //Logic
            e.Invoke();
        }
    }
}
