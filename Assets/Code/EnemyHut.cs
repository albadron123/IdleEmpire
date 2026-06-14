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

public enum EnemyBuildingSpawnerType
{
    Classic,
    Resource,
    Mixed
}

class EnemyHut : MonoBehaviour
{
    public EnemyBuildingType type;
    public EnemyBuildingSpawnerType spawnerType = EnemyBuildingSpawnerType.Classic;

    public GameObject enemyGroup = null;

    public float timeToAttack = 20;


    [SerializeField]
    private TMPro.TMP_Text timerText;
    private Animator a;

    [Header("Shooting variables")]
    [SerializeField] Transform shootingPoint;
    [SerializeField] GameObject projectilePrefab;


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
        float projectileSize = 1.25f;
        int damage = 10;
        while (true)
        {
            GameObject nearestTower = null;
            do
            {
                yield return new WaitForSeconds(timeToAttack);
                nearestTower = MaximUtils.GetNearestWithTag(shootingPoint.position, CoreGame.TAG_BUILDING);
            } while (nearestTower == null);

            Collider2D enemyCollider = nearestTower.GetComponent<Collider2D>();
            if (enemyCollider == null)
            {
                Debug.LogError("Tower Doesnt have a collider");
            }
            Vector2 enemyShootingPosition = enemyCollider.offset + (Vector2)nearestTower.transform.position;
            Vector3 position = shootingPoint.position + new Vector3(0, 0, -9);
            Vector3 direction = (enemyShootingPosition - (Vector2)position).normalized;
            Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            CoreGame.inst.ShootProjectile(
                            projectilePfb: projectilePrefab,
                            projectilePosition: shootingPoint.position + new Vector3(0, 0, -9),
                            destroyTime: 2f,
                            direction: direction,
                            rotation: rotation,
                            damage: damage,
                            projectileSize: projectileSize,
                            doAffectBlobs: false);
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

