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
    [SerializeField] TMPro.TMP_Text timeStats;
    [SerializeField] TMPro.TMP_Text bonesStats;

    [Header("Bones Slider")]
    [SerializeField] Transform bonesSpentSlider;
    [SerializeField] Transform bonesSpentArrow;
    [SerializeField] TMPro.TMP_Text bonesSpentTe;
    [SerializeField] TMPro.TMP_Text bonesTillMaxTe;


    [SerializeField] SpriteRenderer fadeOut;

    enum Status { normal, dialogue, sign};


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

        ViewTimeStats(G.lastRoundLength);
        ViewBonesStats(G.lastRoundBones);

        InitBonesSpentSlider();

        //Action steps
        devilSr.DOFade(2, 1).SetEase(Ease.InQuint);
        yield return new WaitForSeconds(1);

        scroll.transform.DOMoveY(-0.15f, 2).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UpdateBonesSpentSlider());

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
        bonesSpentTe.text = $"{G.lastRoundBones} <sprite=3>";
    }

    public void PressRetry()
    {
        StartCoroutine(LoadScene(G.SCENE_MAIN));
    }

    public void PressCampfire()
    {   
        StartCoroutine(LoadScene(G.SCENE_META));
        
    }


    IEnumerator PressAssignButton()
    {
        yield return new WaitForSeconds(0.1f);
        
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

    public IEnumerator UpdateBonesSpentSlider()
    {
        const float UPDATE_TIME = 0.75f;


        int difference = G.lastRoundBones;
        int oldBonesSpent = G.bonesInProgressBar;
        int maxBones = DataStorage.metagameVariables.bonesPerContract[G.contractNo];
        int newBones = Mathf.Clamp(oldBonesSpent+difference, 0, maxBones);
        G.SaveBonesInProgressbar(newBones);
        float fraction = (float)newBones / maxBones;
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
                bonesSpentTe.text = $"{newBones} <sprite=3>";
                if (newBones != maxBones)
                {
                    bonesTillMaxTe.text = $"Sign <size=3>(need {maxBones - newBones} <sprite=3> more)</size>";
                }
            }
            else
            {
                bonesSpentTe.text = $"{(int)(Mathf.Lerp(oldBonesSpent, newBones, timer / UPDATE_TIME))} <sprite=3>";
                if (newBones != maxBones)
                {
                    bonesTillMaxTe.text = $"Sign <size=3>(need {(int)Mathf.Lerp(maxBones - oldBonesSpent, maxBones - newBones, timer / UPDATE_TIME)} <sprite=3> more)</size>";
                }
            }
        }
    }

    private void ViewTimeStats(int timeInSeconds)
    {
        int minutes = (int)((float)timeInSeconds / 60);
        int seconds = timeInSeconds - minutes * 60;
        timeStats.text = $"<size=9>{minutes}:{seconds}</size> min. survived";
    }

    private void ViewBonesStats(int bonesCount)
    {
        G.ShortenBigNumber(bonesCount);
        bonesStats.text = $"<size=9>+{G.ShortenBigNumber(bonesCount)}</size>  <sprite=3> aquired";
    }
}
