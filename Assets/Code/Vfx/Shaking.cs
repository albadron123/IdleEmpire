using UnityEngine;
using DG.Tweening;
class Shaking : MonoBehaviour
{
    private void Start()
    {
        transform.DOShakePosition(3, 0.1f, 30, 90, false, false).SetLoops(-1, LoopType.Restart);        
    }
}

