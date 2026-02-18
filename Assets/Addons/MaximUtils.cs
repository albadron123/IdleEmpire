using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaximUtils
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
}
