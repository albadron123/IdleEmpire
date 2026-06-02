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
    [SerializeField] GameObject retryButton;
    [SerializeField] TMPro.TMP_Text timeStats;
    [SerializeField] TMPro.TMP_Text bonesStats;
    [SerializeField] GameObject tempInteractableArea;

    [Header("Bones Slider")]
    [SerializeField] Transform bonesSpentSlider;
    [SerializeField] Transform bonesSpentArrow;
    [SerializeField] TMPro.TMP_Text bonesSpentTe;
    [SerializeField] TMPro.TMP_Text bonesTillMaxTe;

    [Header("Sign button")]
    [SerializeField] GameObject signButton;
    [SerializeField] TMPro.TMP_Text signButtonTe;
    [SerializeField] SpriteRenderer signButtonSr;
    [SerializeField] ParticleSystem signButtonPs;
    [SerializeField] Material glowButtonMaterial;
    [SerializeField] Material defaultButtonMaterial;
    
    [Header("Fade out")]
    [SerializeField] SpriteRenderer fadeOut;
    [SerializeField] SpriteRenderer fadeOutWhite;


    private Coroutine colorCoroutine = null;

    bool maxedSlider = false;

    private int devilLineId = 0;

    enum Status { normal, dialogue, sign};


    void Start()
    {
        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        //Calculate Bones
        int difference = G.lastRoundBones;
        int oldBones = G.bonesInProgressBar;
        int maxBones = DataStorage.metagameVariables.bonesPerContract[G.contractNo];
        int newBones = Mathf.Clamp(oldBones + difference, 0, maxBones);

        if (!G.watchedInitialDialogueWithDevil)
        {
            tempInteractableArea.SetActive(true);
            //This code is not to get division by zero exception
            if (G.lastRoundBones == 0)
            {
                G.lastRoundBones = 1;
                G.bonesOnBalance++;
            }
            newBones = G.lastRoundBones;
            maxBones = newBones;
        }
        else
        {
            tempInteractableArea.SetActive(false);
            G.SaveBonesInProgressbar(newBones);
        }

        maxedSlider = (newBones == maxBones);

        //Set up the 'Sign button' visual 
        if (maxedSlider)
        {
            ViewSignButtonActivated();
        }
        else
        {
            ViewSignButtonDefault();
        }


        //Initialize step
        SpriteRenderer devilSr = devil.GetComponent<SpriteRenderer>();
        devilSr.color = new Color(devilSr.color.r, devilSr.color.g, devilSr.color.b, 0);
        scroll.transform.position += 10 * Vector3.down;

        if (!G.watchedInitialDialogueWithDevil)
        {
            if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = StartCoroutine(WriteDialogueLine(DataStorage.introDevilLines[devilLineId], 1.45f, 1f, Ease.OutCubic));
            ++devilLineId;
        }
        else
        {
            if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = StartCoroutine(WriteDialogueLine("Hello World", 1.45f, 1f, Ease.OutCubic));
        }


        retryButton.SetActive(false);
        upgradeTreeButton.SetActive(false);

        ViewTimeStats(G.lastRoundLength);
        ViewBonesStats(G.lastRoundBones);

        InitBonesSpentSlider();

        //Action steps
        devilSr.DOFade(2, 1).SetEase(Ease.InQuint);
        yield return new WaitForSeconds(1);

        if (G.watchedInitialDialogueWithDevil)
        {
            yield return ViewContract(oldBones, newBones, maxBones);
        }
    }


    IEnumerator ViewContract(int oldBones, int newBones, int maxBones)
    {
        scroll.transform.DOMoveY(-0.15f, 2).SetEase(Ease.OutQuint);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(UpdateBonesSpentSlider(oldBones, newBones, maxBones));

        yield return new WaitForSeconds(0.85f);


        if (!maxedSlider)
        {
            ShowControlButtons();
        }
    }

    private Coroutine dialogueCoroutine = null;
    private Sequence dialogueTween = null;
    public IEnumerator WriteDialogueLine(string line, float atTime, float duration, Ease ease)
    {
        dialogueTween?.Kill();
        dialogueSr.color = new Color(dialogueSr.color.r, dialogueSr.color.g, dialogueSr.color.b, 0);
        dialogueTe.color = new Color(dialogueTe.color.r, dialogueTe.color.g, dialogueTe.color.b, 0);
        dialogueWindow.localScale = Vector3.zero;
        dialogueWindow.localRotation = Quaternion.Euler(0, 0, 70);
        dialogueTe.text = line;
        yield return new WaitForSeconds(atTime);

        dialogueTween = DOTween.Sequence()
            .Append(dialogueSr.DOFade(1, duration).SetEase(ease))
            .Join(dialogueTe.DOFade(1, duration).SetEase(ease))
            .Join(dialogueWindow.DOScale(1, duration).SetEase(ease))
            .Join(dialogueWindow.DOLocalRotate(new Vector3(0,0,-5), duration).SetEase(ease));
    }

    private void ShowControlButtons()
    {
        retryButton.SetActive(true);
        SpriteRenderer retryButtonSr = retryButton.GetComponent<SpriteRenderer>();
        retryButtonSr.color = new Color(0, 0, 0, 0);
        retryButtonSr.DOColor(new Color(1, 1, 1, 1), 0.75f);

        upgradeTreeButton.SetActive(true);
        SpriteRenderer upgradeTreeButtonSr = upgradeTreeButton.GetComponent<SpriteRenderer>();
        upgradeTreeButtonSr.color = new Color(0, 0, 0, 0);
        upgradeTreeButtonSr.DOColor(new Color(1, 1, 1, 1), 0.75f);
    }


    private IEnumerator GradientTheText(TMPro.TMP_Text te, Color aTop, Color bTop, Color aBottom, Color bBottom)
    {
        float timer = 0;
        int timerDirection = 1;
        while (true)
        {
            Color topC = Color.Lerp(aTop, bTop, timer);
            Color bottomC = Color.Lerp(aBottom, bBottom, timer);
            te.colorGradient = new TMPro.VertexGradient(topC, topC, bottomC, bottomC);
            timer += Time.deltaTime * timerDirection;
            if (timer > 1)
            {
                timer = 1;
                timerDirection = -timerDirection;
            }
            if (timer < 0)
            {
                timer = 1;
                timerDirection = -timerDirection;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
    void Update()
    {
        MaximUtils.RenderShakyText(dialogueTe, 0.012f, 15);
        if (maxedSlider)
        {
            MaximUtils.RenderWavyText(signButtonTe, 0.07f);
        }
    }

    public void PrintDialogueLine(string providedLine = "")
    {
        string line = providedLine;
        if (line == "")
        {
            if (!G.watchedInitialDialogueWithDevil)
            {
                if (devilLineId >= DataStorage.introDevilLines.Length)
                {
                    line = DataStorage.introDevilLines[DataStorage.introDevilLines.Length - 1];
                }
                else
                {
                    line = DataStorage.introDevilLines[devilLineId];
                    ++devilLineId;
                    if (devilLineId == DataStorage.introDevilLines.Length)
                    {
                        tempInteractableArea.SetActive(false);
                        StartCoroutine(ViewContract(0, G.lastRoundBones, G.lastRoundBones));
                    }
                }
            }
            else
            {
                line = "You give me <color=red>BONES</color>\nI give you <color=red>POWER</color>";
            }
        }
        if (dialogueCoroutine != null) StopCoroutine(dialogueCoroutine);
        dialogueCoroutine = StartCoroutine(WriteDialogueLine(line, 0, 0.75f, Ease.OutCubic));
    }

    void ViewSignButtonActivated()
    {
        colorCoroutine = StartCoroutine(GradientTheText(signButtonTe, new Color(1, 0, 1), new Color(1, 0, 0), new Color(0.6f, 0, 0), new Color(0.6f, 0, 1)));
        signButtonSr.material = glowButtonMaterial;
        signButton.transform.DOShakePosition(1, 0.05f, 50, 90, false, false).SetLoops(-1);
    }

    void ViewSignButtonDefault()
    {
        signButton.transform.DOKill();
        signButtonTe.enableVertexGradient = false;
        signButtonTe.color = new Color(1, 1, 1, 0.5f);
        signButtonSr.color = new Color(1, 1, 1, 0.5f);
        signButtonSr.material = defaultButtonMaterial;
    }


    void InitBonesSpentSlider()
    {
        bonesSpentSlider.localScale = new Vector3(0, bonesSpentSlider.localScale.y, bonesSpentSlider.localScale.z);
        bonesSpentArrow.localPosition = new Vector3(0, bonesSpentArrow.transform.localPosition.y, bonesSpentArrow.transform.localPosition.z);
        bonesSpentTe.text = $"0 <sprite=3>";
    }

    public void PressRetry()
    {
        StartCoroutine(LoadScene(G.SCENE_MAIN));
    }

    public void PressCampfire()
    {   
        StartCoroutine(LoadScene(G.SCENE_META));
        
    }

    public void PressSign(Interactable i)
    {
        if (!maxedSlider)
        {
            i.PerformCancelAction();
            return;
        }
        else
        {
            StartCoroutine(PressSignButtonEffectively());
        }
    }

    IEnumerator PressSignButtonEffectively()
    {
        signButtonPs.gameObject.SetActive(true);
        signButtonPs.Play();
        //vfx
        yield return signButton.transform.DOShakePosition(0.5f, 0.09f, 50, 90, false, false).WaitForCompletion();
        yield return signButton.transform.DOShakePosition(0.8f, 0.12f, 50, 90, false, false).WaitForCompletion();
        signButton.transform.DOShakePosition(1f, 0.15f, 50, 90, false, false).SetLoops(-1);
        yield return fadeOutWhite.DOFade(1, 0.4f).SetEase(Ease.InCubic).WaitForCompletion();
        //Cancel visual effects
        signButtonPs.Stop();
        ViewSignButtonDefault();
        maxedSlider = false;
        MaximUtils.RenderWavyText(signButtonTe, 0);
        //Wait
        yield return new WaitForSeconds(0.8f);
        
        //Set the dialogue line
        if (!G.watchedInitialDialogueWithDevil)
        {
            PrintDialogueLine("<color=red>DEAL!</color>\nNow, choose your rewards at the campfire.");
            //we have seen the dialogue, now we can save this fact
            G.SavePassedInitialScene();
        }
        else
        {
            PrintDialogueLine("You ascended, so did your enemies");
            G.SaveContractNo(++G.contractNo);
        }

        if (G.contractNo >= DataStorage.metagameVariables.contractsCount)
        {
            PrintDialogueLine("Game is completed. Load final scene here.");
        }
        else
        {
            //Reinitialize the slider
            G.SaveBonesInProgressbar(0);
            bonesTillMaxTe.text = $"Sign <size=3>(need {DataStorage.metagameVariables.bonesPerContract[G.contractNo]} <sprite=3> more)</size>";
            InitBonesSpentSlider();
        }

        //Reset to default
        yield return fadeOutWhite.DOFade(0, 0.2f).WaitForCompletion();
        ShowControlButtons();
        //Vfx
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

    public IEnumerator UpdateBonesSpentSlider(int oldBones, int newBones, int maxBones)
    {
        const float UPDATE_TIME = 0.75f;

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
                bonesSpentTe.text = $"{(int)(Mathf.Lerp(oldBones, newBones, timer / UPDATE_TIME))} <sprite=3>";
                if (newBones != maxBones)
                {
                    bonesTillMaxTe.text = $"Sign <size=3>(need {(int)Mathf.Lerp(maxBones - oldBones, maxBones - newBones, timer / UPDATE_TIME)} <sprite=3> more)</size>";
                }
            }
        }
    }

    private void ViewTimeStats(int timeInSeconds)
    {
        int minutes = (int)((float)timeInSeconds / 60);
        int seconds = timeInSeconds - minutes * 60;
        timeStats.text = $"<size=9>{minutes:D2}:{seconds:D2}</size> min. survived";
    }

    private void ViewBonesStats(int bonesCount)
    {
        G.ShortenBigNumber(bonesCount);
        bonesStats.text = $"<size=9>+{G.ShortenBigNumber(bonesCount)}</size>  <sprite=3> aquired";
    }
}
