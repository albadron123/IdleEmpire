using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class EnemyHut : MonoBehaviour
{
    public GameObject enemyGroup = null;
    
    int timeToAttack = 20;

    
    [SerializeField] 
    private TMPro.TMP_Text timerText;
    private Animator a;


    private void Start()
    {
        a = GetComponent<Animator>();
        StartCoroutine(StartAttack());
    }

    IEnumerator StartAttack()
    {
        while (timeToAttack != 0)
        {
            timerText.text = timeToAttack.ToString().Bold() + "sec".Size(3.7f);
            yield return new WaitForSeconds(1);
            --timeToAttack;
        }
        timerText.text = "Attack!";
        Instantiate(enemyGroup, transform.position+new Vector3(0,0,0.05f),Quaternion.identity);
        a.SetBool("destroy", true);
    }

    public void DestoyHut()
    {
        Destroy(gameObject);
    }

}

