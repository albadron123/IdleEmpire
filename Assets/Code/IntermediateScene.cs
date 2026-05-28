using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IntermediateScene : MonoBehaviour
{
    [SerializeField] GameObject devil;
    [SerializeField] GameObject scroll;
    
    [SerializeField] Transform dialogueWindow;
    [SerializeField] SpriteRenderer dialogueSr;
    [SerializeField] TMPro.TMP_Text dialogueTe;
    [SerializeField] GameObject upgradeTreeButton;
    [SerializeField] GameObject retryAgain;
    [SerializeField] TMPro.TMP_Text stats;

    [Header("Bones Slider")]
    [SerializeField] Transform bonesSpentSlider;
    [SerializeField] Transform bonesSpentArrow;
    [SerializeField] TMPro.TMP_Text bonesSpentTe;
    [SerializeField] TMPro.TMP_Text bonesTillMaxTe;


    [SerializeField] SpriteRenderer fadeOut;


    void Start()
    {
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        //Initialize step
        SpriteRenderer devilSr = devil.GetComponent<SpriteRenderer>();
        devilSr.color = new Color(devilSr.color.r, devilSr.color.g, devilSr.color.b, 0);
        scroll.transform.position += 10 * Vector3.down;

        dialogueSr.color = new Color(dialogueSr.color.r, dialogueSr.color.g, dialogueSr.color.b, 0);
        dialogueTe.color = new Color(dialogueTe.color.r, dialogueTe.color.g, dialogueTe.color.b, 0);
        dialogueWindow.localScale = Vector3.zero; 

        InitBonesSpentSlider();

        //Action steps
        devilSr.DOFade(2, 1).SetEase(Ease.InQuint);
        yield return new WaitForSeconds(1);

        scroll.transform.DOMoveY(-0.15f, 2).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UpdateBonesSpentSlider(57));

        yield return new WaitForSeconds(0.75f);
        DOTween.Sequence()
            .Append(dialogueSr.DOFade(1, 1).SetEase(Ease.InQuint))
            .Join(dialogueTe.DOFade(1, 1).SetEase(Ease.InQuint))
            .Join(dialogueWindow.DOScale(1, 1));
    }

    
    void Update()
    {
        MaximUtils.RenderShakyText(dialogueTe, 0.012f, 15);
    }


    void InitBonesSpentSlider()
    {
        bonesSpentSlider.localScale = new Vector3(0, bonesSpentSlider.localScale.y, bonesSpentSlider.localScale.z);
        bonesSpentArrow.localPosition = new Vector3(0, bonesSpentArrow.transform.localPosition.y, bonesSpentArrow.transform.localPosition.z);
        bonesSpentTe.text = $"{G.bonesSpent} <sprite=3>";

        //StartCoroutine(UpdateBonesSpentSlider(0));
    }

    public void PressRetry()
    {
        StartCoroutine(LoadScene(G.SCENE_MAIN));
    }

    public void PressCampfire()
    {   
        StartCoroutine(LoadScene(G.SCENE_META));
        
    }

    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        fadeOut.DOFade(1, 0.3f);
        ao.allowSceneActivation = false;
        yield return new WaitForSeconds(0.4f);
        if (sceneName == G.SCENE_META)
        {
            SoundManager.inst.PlaySfx(DataStorage.SFX_PIANO_LOW, 0.25f);
        }
        ao.allowSceneActivation = true;
        yield return ao;
    }

    public IEnumerator UpdateBonesSpentSlider(int newBonesSpent)
    {
        const float UPDATE_TIME = 0.75f;

        int oldBonesSpent = G.bonesSpent;
        G.bonesSpent = Mathf.Clamp(newBonesSpent, 0, G.maxBonesSpent);
        float fraction = (float)G.bonesSpent / G.maxBonesSpent;
        bonesSpentSlider.DOScale(new Vector3(fraction, bonesSpentSlider.localScale.y, bonesSpentSlider.localScale.z), UPDATE_TIME).SetEase(Ease.OutSine);
        bonesSpentArrow.DOLocalMoveX(4.55f*fraction, UPDATE_TIME).SetEase(Ease.OutSine);
        float timer = 0;
        while (timer < UPDATE_TIME)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            if (timer >= UPDATE_TIME)
            {
                timer = UPDATE_TIME;
                bonesSpentTe.text = $"{G.bonesSpent} <sprite=3>";
                if (G.maxBonesSpent != G.bonesSpent)
                {
                    bonesTillMaxTe.text = $"Sign <size=3>(need {G.maxBonesSpent - G.bonesSpent} <sprite=3> more)</size>";
                }
            }
            else
            {
                bonesSpentTe.text = $"{(int)(Mathf.Lerp(oldBonesSpent, G.bonesSpent, timer / UPDATE_TIME))} <sprite=3>";
                if (G.maxBonesSpent != G.bonesSpent)
                {
                    bonesTillMaxTe.text = $"Sign <size=3>(need {(int)Mathf.Lerp(G.maxBonesSpent - oldBonesSpent, G.maxBonesSpent - G.bonesSpent, timer / UPDATE_TIME)} <sprite=3> more)</size>";
                }
            }
        }
    }
}
