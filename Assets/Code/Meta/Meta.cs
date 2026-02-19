using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Meta : MonoBehaviour
{
    public static Meta inst = null;

    [HideInInspector]
    public Node currentNode = null;
    

    void Start()
    {
        if (inst != null)
        {
            Debug.LogError("Meta.cs is singleton!");
            Destroy(this);
        }
        else
        {
            inst = this;
        }
    }

    public void StartAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
