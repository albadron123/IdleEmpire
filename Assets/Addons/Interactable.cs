using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Interactable : MonoBehaviour
{
    public bool isInteractive = true;
    
    [SerializeField]
    UnityEvent e;

    [SerializeField]
    UnityEvent mouseEnterCustomEvent;
    [SerializeField]
    UnityEvent mouseExitCustomEvent;


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


    private void OnMouseEnter()
    {
        if (lastTween != null)
        {
            lastTween.Kill();
        }
        lastTween = DOTween.Sequence();
        lastTween.Append(t.DOScale(1.03f * initialScale, 0.2f));

        mouseEnterCustomEvent.Invoke();
    }

    private void OnMouseExit()
    {
        if (lastTween != null)
        {
            lastTween.Kill();
        }
        lastTween = DOTween.Sequence();
        lastTween.Append(t.DOScale(1f * initialScale, 0.2f));

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
            lastTween = DOTween.Sequence();
            lastTween.Append(t.DOScale(0.95f * initialScale, 0.1f));
            lastTween.Append(t.DOScale(1.03f * initialScale, 0.1f));
        
            //Logic
            e.Invoke();
        }
    }
}
