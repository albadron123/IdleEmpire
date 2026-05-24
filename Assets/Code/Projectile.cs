using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : MonoBehaviour
{

    public List<GameObject> ignoreList = new List<GameObject>();
    public bool doAffectBlobs = true;

    public Vector3 direction = Vector3.right;
    public float velocity = 2;
    public int damage = 1;
    public float time;

    public float size;

    private Transform t;

    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        t.localScale = new Vector3(0, 0, 1);
    }

    public IEnumerator ProjectileLifeCycle(float lifetime)
    {
        t = transform;
        t.DOScale(new Vector3(size, size), lifetime * 0.1f);
        yield return new WaitForSeconds(lifetime * 0.9f);
        t.DOScale(0, lifetime * 0.1f).SetEase(Ease.Flash);
        yield return new WaitForSeconds(lifetime * 0.1f);

        Die();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * Time.deltaTime * velocity;
    }

    public void Die()
    {
        GameObject inst = Instantiate(CoreGame.inst.projectileDeathPlacePfb, t.position, Quaternion.identity);
        SpriteRenderer sr = inst.GetComponent<SpriteRenderer>();
        DOTween.Sequence()
            .Append(sr.DOFade(0.3f, 1f))
            .Join(t.DOScale(0.1f, 1f));
        Destroy(inst, 1f);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        CoreGame.inst.PlayContactEffect(t.position);
    }
}
