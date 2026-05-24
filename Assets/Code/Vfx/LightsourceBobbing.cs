using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LightsourceBobbing : MonoBehaviour
{
    [SerializeField] float min;
    [SerializeField] float max;
    Transform t;
    SpriteRenderer sr;

    Vector3 initialScale;

    void Start()
    {
        t = transform;
        initialScale = t.localScale;
        sr = GetComponent<SpriteRenderer>();
        
        transform.DOScale(max * initialScale, 1.5f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);

        
        sr.DOFade(0.75f, 1f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
