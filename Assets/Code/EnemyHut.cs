using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EnemyBuildingType
{
    Hut,
    Tower,
    Shooting,
    Static
}

class EnemyHut : MonoBehaviour
{
    public EnemyBuildingType type;

    public GameObject enemyGroup = null;

    public float timeToAttack = 20;


    [SerializeField]
    private TMPro.TMP_Text timerText;
    private Animator a;


    private void Start()
    {
        a = GetComponent<Animator>();
        if (type == EnemyBuildingType.Hut)
        {
            StartCoroutine(StartAttack());
        }
        if (type == EnemyBuildingType.Shooting)
        {
            timerText.text = "";
            StartCoroutine(Shooting());
        }
        if (type == EnemyBuildingType.Tower)
        {
            StartCoroutine(SpawningWaves());
        }
        if (type == EnemyBuildingType.Static)
        {
            timerText.text = "";
        }
    }

    IEnumerator StartAttack()
    {
        while (timeToAttack > 0)
        {
            timerText.text = ((int)timeToAttack).ToString().Bold() + "sec".Size(3.7f);
            yield return new WaitForSeconds(1);
            timeToAttack -= 1;
        }
        timerText.text = "Attack!";
        Instantiate(enemyGroup, transform.position + new Vector3(0, 0, 0.05f), Quaternion.identity);
        a.SetBool("destroy", true);
    }

    IEnumerator Shooting()
    {
        while (true)
        {
            yield return timeToAttack;
            Debug.Log("Shoot something");
        }
    }

    IEnumerator SpawningWaves()
    {
        float timerToAttack = 0;
        while (true)
        {
            timerToAttack = timeToAttack;
            while (timerToAttack > 0)
            {
                timerText.text = ((int)timerToAttack).ToString().Bold() + "sec".Size(3.7f);
                yield return new WaitForSeconds(1);
                timerToAttack -= 1;
            }
            Instantiate(enemyGroup, transform.position + new Vector3(0, 0, 0.05f), Quaternion.identity);
        }
    }

    public void DestoyHut()
    {
        Destroy(gameObject);
    }

}

