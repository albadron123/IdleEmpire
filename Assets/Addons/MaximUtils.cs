using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using DG.Tweening;

public class MaximUtils : MonoBehaviour
{

    public static List<int> RandomPermutations(int from, int to)
    {
        //Shuffles a list of numbers [from, from+1, ..., to-1, to]


        List<int> permutations = new List<int>();
        for (int i = from; i <= to; ++i)
        {
            permutations.Add(i);
        }

        //Shuffle (fisher-yates shuffle)
        for (int i = permutations.Count-1; i > 0; i--)
        {
            int j = Random.Range(0, i);
            int t = permutations[i];
            permutations[i] = permutations[j];
            permutations[j] = t;
        }

        return permutations;
    }
    public static List<int> RandomIndicesUnique(int indexMax, int count)
    {
        List<int> randomIds = new List<int>();
        if (indexMax < count)
        {
            Debug.LogError("Can't create a given amount of unique indexes till indexMax");
            return randomIds;
        }
        for (int i = 0; i < count; ++i)
        {
            int newId;
            int iters = 0;
            do
            {
                newId = Random.Range(0, indexMax);
                ++iters;
            }
            while (randomIds.Contains(newId) && iters < 10_000_000);
            if (iters >= 10_000_000)
            {
                Debug.LogError("Can't create a given amount of unique indexes till indexMax");
                return randomIds;
            }
            randomIds.Add(newId);
        }
        return randomIds;
    }

    /// <summary>
    /// Returns a random dot inside the frame with bounds inner and outer. IMPORTANT!: inner should be contained inside outer. IMPORTANT!: X,Y are the lower-left X,Y.
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="outer"></param>
    /// <returns></returns>
    public static UnityEngine.Vector2 RandomPositionInsideFrame(Rect inner, Rect outer)
    {
        int part = Random.Range(0, 4);
        switch (part)
        {
            case 0: return new UnityEngine.Vector2(Random.Range(outer.x, inner.x), Random.Range(outer.y, outer.y + outer.height));
            case 1: return new UnityEngine.Vector2(Random.Range(inner.x + inner.width, outer.x + outer.width), Random.Range(outer.y, outer.y + outer.height));
            case 2: return new UnityEngine.Vector2(Random.Range(inner.x, inner.x + inner.width), Random.Range(outer.y, inner.x));
            case 3: return new UnityEngine.Vector2(Random.Range(inner.x, inner.x + inner.width), Random.Range(inner.y + inner.height, outer.y + outer.height));
        }
        // unreachable
        Debug.LogError("An unreached section is reached in MaximUtils function");
        return UnityEngine.Vector2.zero;
    }


    public static bool MouseOverCollider(Collider2D col, float radius)
    {
        Vector2 mousePosition = MousePosition();
        Collider2D[] overlapped = Physics2D.OverlapCircleAll(mousePosition, radius);
        foreach (var overlappedCol in overlapped)
        {
            if(overlappedCol == col)
            {
                return true;
            }
        }
        return false;
    }

    public static Vector2 MousePosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    

    public static bool DoIOverlap(Collider2D col)
    {
        List<Collider2D> overlapped = new List<Collider2D>();
        Physics2D.OverlapCollider(col, new ContactFilter2D().NoFilter(), overlapped);
        foreach (Collider2D other in overlapped)
        {
                return true;
        }
        return false;
    }

    public static bool DoIOverlapAndMatch(Collider2D col, System.Predicate<Collider2D> predicate)
    {
        List<Collider2D> overlapped = new List<Collider2D>();
        Physics2D.OverlapCollider(col, new ContactFilter2D().NoFilter(), overlapped);
        foreach (Collider2D other in overlapped)
        {
            if (predicate(other))
            {
                return true;
            }
        }
        return false;
    }

    public static bool DoIOverlapTag2D(Collider2D col, string tag)
    {
        return DoIOverlapAndMatch(col, (col) => col.gameObject.tag == tag);
    }

    public static Collider2D GetAnyOverlappedWithTag2D(Collider2D col, string tag)
    {
        List<Collider2D> overlapped = new List<Collider2D>();
        Physics2D.OverlapCollider(col, new ContactFilter2D().NoFilter(), overlapped);
        foreach (Collider2D other in overlapped)
        {
            if (other.gameObject.CompareTag(tag))
            {
                return other;
            }
        }
        return null;
    }

    public static List<Collider2D> GetAllOverlappedWithTag2D(Collider2D col, string tag)
    {
        List<Collider2D> overlapped = new List<Collider2D>();
        List<Collider2D> result = new List<Collider2D>();
        Physics2D.OverlapCollider(col, new ContactFilter2D().NoFilter(), overlapped);
        foreach (Collider2D other in overlapped)
        {
            if (other.gameObject.CompareTag(tag))
            {
                result.Add(other);
            }
        }
        return result;
    }

    public static List<Collider2D> GetAllOverlappedWithTag2D(UnityEngine.Vector3 position, float radius, string tag)
    {
        Collider2D[] overlapped = Physics2D.OverlapCircleAll(position, radius);
        List<Collider2D> result = new List<Collider2D>();
        foreach (Collider2D col in overlapped)
        {
            if (col.gameObject.CompareTag(tag))
            {
                result.Add(col);
            }
        }
        return result;
    }



    public static Collider2D GetNearestOverlappedWithTag2D(Collider2D col, string tag)
    {
        List<Collider2D> overlapped = new List<Collider2D>();
        Physics2D.OverlapCollider(col, new ContactFilter2D().NoFilter(), overlapped);
        Collider2D nearest = null;
        float shortestDistance = float.MaxValue;
        foreach (Collider2D other in overlapped)
        {
            if (other.gameObject.CompareTag(tag))
            {
                if (nearest == null)
                {
                    nearest = other;
                    shortestDistance = UnityEngine.Vector2.Distance(col.transform.position, other.transform.position);
                }
                else
                {
                    float currentDistance = UnityEngine.Vector2.Distance(col.transform.position, other.transform.position);
                    if (currentDistance < shortestDistance)
                    {
                        nearest = other;
                        shortestDistance = currentDistance;
                    }
                }
            }
        }
        return nearest;
    }

    public static bool DoSquareOverlapAny(UnityEngine.Vector2 position, UnityEngine.Vector2 size)
    {
        Collider2D overlapped = Physics2D.OverlapBox(position, size, 0f);
        return overlapped != null;
    }

    public static Collider2D GetNearestOverlappedWithTag2D(UnityEngine.Vector2 point, float radius, string tag)
    {
        Collider2D[] overlapped;
        overlapped = Physics2D.OverlapCircleAll(point, radius);
        Collider2D nearest = null;
        float shortestDistance = float.MaxValue;
        foreach (Collider2D other in overlapped)
        {
            if (other.gameObject.CompareTag(tag))
            {
                if (nearest == null)
                {
                    nearest = other;
                    shortestDistance = UnityEngine.Vector2.Distance(point, other.transform.position);
                }
                else
                {
                    float currentDistance = UnityEngine.Vector2.Distance(point, other.transform.position);
                    if (currentDistance < shortestDistance)
                    {
                        nearest = other;
                        shortestDistance = currentDistance;
                    }
                }
            }
        }
        return nearest;
    }

    public static GameObject GetNearestWithTag(UnityEngine.Vector2 point, string tag)
    {
        List<GameObject> objs = GameObject.FindGameObjectsWithTag(tag).ToList<GameObject>();
        if(objs == null || objs.Count == 0)
        {
            return null;
        }
        objs.Sort(
            (GameObject x, GameObject y) =>  
                UnityEngine.Vector2.Distance(x.transform.position, point)>
                UnityEngine.Vector2.Distance(x.transform.position, point)?
                1:-1);
        return objs[0];
    }



    // USE ONLY WHEN YOU DONT NEED OBJECTS ANYMORE! CAN BE INEFFICIENT!
    public static int CountGameObjectsWithTag(string tag)
    {
        return GameObject.FindGameObjectsWithTag(tag).Length;
    }



    public static List<GameObject> DrawCenteredListHor(GameObject obj, Transform container, UnityEngine.Vector3 center, float delta, int count, float widthMult)
    {
        List<GameObject> instances = new List<GameObject>();

        float width = obj.transform.localScale.x * widthMult;
        float length = count * width + (count - 1) * delta;
        UnityEngine.Vector3 begin = center + new UnityEngine.Vector3(-length / 2f + width / 2f, 0, 0);
        UnityEngine.Vector3 diff = new UnityEngine.Vector3(width + delta, 0, 0);

        for (int i = 0; i < count; ++i)
        {
            GameObject inst = Instantiate(obj, UnityEngine.Vector3.zero, UnityEngine.Quaternion.identity, container);
            inst.transform.localPosition = begin + diff * i;
            instances.Add(inst);
        }
        return instances;
    }


    public static void DOCancelShake(Transform t)
    {
        t.DOKill();
        Sequence seq = DOTween.Sequence();
        seq.Append(t.DORotate(new UnityEngine.Vector3(0, 0, 5), 0.04f));
        seq.Append(t.DORotate(new UnityEngine.Vector3(0, 0, -5), 0.08f));
        seq.Append(t.DORotate(new UnityEngine.Vector3(0, 0, 0), 0.04f));
        seq.SetLoops(2);
        seq.OnKill(() => { t.rotation = UnityEngine.Quaternion.identity; });
    }

    public static UnityEngine.Vector2 RandomVector2(float maxMagnitude)
    {
        return new UnityEngine.Vector2(Random.Range(-1, 1), Random.Range(-1, 1)).normalized * Random.Range(-maxMagnitude, maxMagnitude);
    }

    public static UnityEngine.Vector2 RandomVector2RandomMagnitudeRange(float minMagitude, float maxMagnitude)
    {
        float magnitude = Random.Range(minMagitude, maxMagnitude);
        return new UnityEngine.Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * magnitude * ((Random.value > 0.5f)?1:-1);
    }

    public static UnityEngine.Vector2 RandomVector2FixMagnitude(float magnitude)
    {
        return new UnityEngine.Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * magnitude * ((Random.value > 0.5f)?1:-1);
    }

    public static void RenderWavyText(TMPro.TMP_Text te, float amplitude)
    {
        te.ForceMeshUpdate(); // Ensure the mesh is updated
        TMP_TextInfo teInfo = te.textInfo;
        for (int i = 0; i < teInfo.characterCount; ++i)
        {
            TMP_CharacterInfo charInfo = teInfo.characterInfo[i];
            if (!charInfo.isVisible)
            {
                continue;
            }
            UnityEngine.Vector3[] verts = teInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; ++j)
            {
                verts[charInfo.vertexIndex + j] += new UnityEngine.Vector3(0, amplitude * Mathf.Sin(Time.time * 10f + i * 5), 0);
            }
        }

        for (int i = 0; i < teInfo.meshInfo.Length; ++i)
        {
            teInfo.meshInfo[i].mesh.vertices = teInfo.meshInfo[i].vertices;
            te.UpdateGeometry(teInfo.meshInfo[i].mesh, i);
        }
    }


    public static LineRenderer[] CreateLineRendererBatch(string objectName, int count, Color c, Material m, float howThin, string sortingLayerName = "FORWARD")
    {
        
        LineRenderer[] lrs = new LineRenderer[count];
        
        GameObject obj = new GameObject(objectName);
        for (int i = 0; i < count; ++i)
        {
            GameObject child = new GameObject($"{i}");
            child.transform.parent = obj.transform;
            lrs[i] = child.AddComponent<LineRenderer>();
            lrs[i].material = m;
            lrs[i].startWidth = howThin;
            lrs[i].endWidth = howThin;
            lrs[i].startColor = c;
            lrs[i].endColor = c;
            lrs[i].sortingLayerName = sortingLayerName;
        }
        
        return lrs;
    }

    public static void RenderDashedCircle(LineRenderer[] lrs, UnityEngine.Vector3 pos, float radius, float timeElapsed, int gaps)
    {
        int lrId = 0;
        int dotsCount = 360;

        // half of the circle is gaps and another half in not
        float angleOfAGap = 360.0f / (gaps * 2);

        List<UnityEngine.Vector3> positions = new List<UnityEngine.Vector3>();

        int prevSectorId = 0;
        for (int i = 0; i < dotsCount; ++i)
        {
            //by sector i mean a part of the circle that is a gap or not a gap
            float sectorAngle = i / (float)dotsCount * 360;
            int sectorId = (int)(sectorAngle / angleOfAGap);
            if (sectorId % 2 != 0 && prevSectorId % 2 == 0)
            {
                //add the last position of the drawn part once more
                lrs[lrId].positionCount = positions.Count;
                lrs[lrId].SetPositions(positions.ToArray());
                positions.Clear();
                ++lrId;
                if (lrId >= lrs.Length)
                {
                    return;
                }
            }

            if (sectorId % 2 == 0)
            {
                positions.Add(pos + new UnityEngine.Vector3(Mathf.Cos((sectorAngle + timeElapsed * 100) * Mathf.Deg2Rad), Mathf.Sin((sectorAngle + timeElapsed * 100) * Mathf.Deg2Rad), -8) * radius);
            }


            prevSectorId = sectorId;
        }

        if (prevSectorId == 0)
        {
            //unsaved stuff
            lrs[lrId].positionCount = positions.Count;
            lrs[lrId].SetPositions(positions.ToArray());
            positions.Clear();
            ++lrId;
        }
        while (lrId < lrs.Length)
        {
            lrs[lrId].positionCount = 0;
            ++lrId;
        }
    }


    static float[] rnd = null;
    public static void RenderShakyText(TMPro.TMP_Text te, float amplitude, float power)
    {
        if (rnd == null)
        {
            rnd = new float[10];
            for (int i = 0; i < 10; ++i)
            {
                rnd[i] = Random.value;
            }
        }
        te.ForceMeshUpdate(); // Ensure the mesh is updated
        TMP_TextInfo teInfo = te.textInfo;
        for (int i = 0; i < teInfo.characterCount; ++i)
        {
            TMP_CharacterInfo charInfo = teInfo.characterInfo[i];
            if (!charInfo.isVisible)
            {
                continue;
            }
            UnityEngine.Vector3[] verts = teInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; ++j)
            {
                verts[charInfo.vertexIndex + j] += new UnityEngine.Vector3(amplitude * Mathf.Sin(Time.time * rnd[(i+2) % 10] * power + rnd[(i + 7) % 10] * 5), 
                                                                           amplitude * Mathf.Sin(Time.time * rnd[i % 10] * power + rnd[(i+3)%10]* 5), 
                                                                           0);
            }
        }

        for (int i = 0; i < teInfo.meshInfo.Length; ++i)
        {
            teInfo.meshInfo[i].mesh.vertices = teInfo.meshInfo[i].vertices;
            te.UpdateGeometry(teInfo.meshInfo[i].mesh, i);
        }
    }

    public static IEnumerator AppearAndClearWavyText(TMPro.TMP_Text te, string s, float appearVelocity, float timeToWait, float amplitude)
    {
        te.gameObject.SetActive(true);
        te.text = "";
        int lettersCount = s.Length;

        float timer = 0;

        for (int i = 0; i < lettersCount; ++i)
        {
            te.text = s.Substring(0, i);
            
            for (; timer < appearVelocity; timer += Time.fixedDeltaTime)
            { 
                RenderWavyText(te, amplitude);
                yield return new WaitForFixedUpdate();
            }
            timer -= appearVelocity;
            if (timer > appearVelocity)
            {
                i += (int)(timer / appearVelocity);
                timer -= (int)(timer / appearVelocity);
            }
        }
        te.text = s;

        for (timer = 0; timer < timeToWait; timer += Time.fixedDeltaTime)
        {
            RenderWavyText(te, amplitude);
            yield return new WaitForFixedUpdate();
        }

        timer = 0;
        for (int i = 0; i < lettersCount; ++i)
        {
            te.text = s.Substring(0, lettersCount-i);

            for (; timer < appearVelocity; timer += Time.fixedDeltaTime)
            {
                RenderWavyText(te, amplitude);
                yield return new WaitForFixedUpdate();
            }
            timer -= appearVelocity;
            if (timer > appearVelocity)
            {
                i += (int)(timer / appearVelocity);
                timer -= (int)(timer / appearVelocity);
            }
        }
        te.text = "";

        te.gameObject.SetActive(false);
    }

}

// ===String extention methods===
public static class StringUtils
{
    public static string Bold(this string s)
    {
        return $"<b>{s}</b>";
    }

    public static string Italic(this string s)
    {
        return $"<i>{s}</i>";
    }

    public static string Color(this string s, string color)
    {
        return $"<color={color}>{s}</color>";
    }

    public static string Size(this string s, float fontSize)
    {
        return $"<size={fontSize}>{s}</size>";
    }
}
