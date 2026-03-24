using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Numerics;


public class MaximUtils : MonoBehaviour
{
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


    public static bool DoIOverlapTag2D(Collider2D col, string tag)
    {
        List<Collider2D> overlapped = new List<Collider2D>();
        Physics2D.OverlapCollider(col, new ContactFilter2D().NoFilter(), overlapped);
        foreach (Collider2D other in overlapped)
        {
            if(other.gameObject.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
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
        if(objs == null)
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
}
