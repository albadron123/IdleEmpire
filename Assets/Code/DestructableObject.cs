using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestructableObject : MonoBehaviour
{
    [HideInInspector]
    public int health;
    public int maxHealth;

    
    float sliderMinX = -0.63f;
    float sliderMaxX = 0;


    IDestructable specificDestruction;

    public GameObject sliderContainer;
    [SerializeField]
    Transform sliderT;

    [SerializeField]
    List<SpriteRenderer> srList = new List<SpriteRenderer>();


    TMPro.TMP_Text healthText;

    void Start()
    {
        
        specificDestruction = GetComponent<IDestructable>();
        InitHealth();
    }

    
    void Update()
    {

    }

    public void InitHealth()
    {
        healthText = sliderContainer.GetComponentInChildren<TMPro.TMP_Text>();
        health = maxHealth;
        healthText.text = $"{health}hp";
        sliderT.transform.localPosition = new Vector3(sliderMaxX, 0, sliderT.transform.localPosition.z);
        sliderContainer.SetActive(false);
    }
    

    public void ChangeHealth(int delta)
    {
        if(delta < 0)
        {
            StopCoroutine(BlinkWhite());
            StartCoroutine(BlinkWhite());
        }
        health += delta;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        if (health >= maxHealth)
        {
            health = maxHealth;
            sliderContainer.SetActive(false);
        }
        if (health < maxHealth)
        {
            sliderContainer.SetActive(true);
        }

        healthText.text = $"{health}hp";

        // Display health on the slider
        sliderT.transform.DOKill();
        sliderT.transform.DOLocalMove(new Vector3(GetSliderDestinationX(), 0, sliderT.transform.localPosition.z), 0.5f);
        //Display the hit damage
        specificDestruction.ChangeHealth(delta);
    }


    IEnumerator BlinkWhite()
    {
        if (srList.Count > 0)
        {
            for (int i = 0; i < srList.Count; ++i)
            {
                srList[i].material = CoreGame.inst.allWhiteMaterial;
            }
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i < srList.Count; ++i)
            {
                srList[i].material = CoreGame.inst.spriteDefaultMaterial;
            }
        }
    }


    public void Die()
    {
        specificDestruction.Die();
        Destroy(gameObject);
    }


    public float GetSliderDestinationX()
    {
        float fraction = (float)health / maxHealth;
        float result = sliderMinX + fraction * (sliderMaxX - sliderMinX);
        return result;
    }

}
