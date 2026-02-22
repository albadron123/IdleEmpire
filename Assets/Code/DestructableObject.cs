using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestructableObject : MonoBehaviour
{
    [HideInInspector]
    public int health;
    public int maxHealth;

    [SerializeField]
    float sliderMinX;
    [SerializeField]
    float sliderMaxX;


    IDestructable specificDestruction;

    public GameObject sliderContainer;
    [SerializeField]
    Transform sliderT;


    void Start()
    {
        specificDestruction = GetComponent<IDestructable>();
        InitHealth();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeHealth(-1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeHealth(1);
        }
    }

    public void InitHealth()
    {
        health = maxHealth;
        sliderT.transform.localPosition = new Vector3(sliderMaxX, 0, sliderT.transform.localPosition.z);
        sliderContainer.SetActive(false);
    }
    

    public void ChangeHealth(int delta)
    {
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
        // Display health on the slider
        sliderT.transform.DOKill();
        sliderT.transform.DOLocalMove(new Vector3(GetSliderDestinationX(), 0, sliderT.transform.localPosition.z), 0.5f);
        //Display the hit damage
        specificDestruction.ChangeHealth(delta);
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
