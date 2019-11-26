using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStart : MonoBehaviour
{
    [SerializeField]
    private Animator playAnimator;
    public Action actionPlay;
    [SerializeField]
    private Text txSurvivalTime, txBestTime;

    public void Run()
    {
        string survivalTime = PlayerPrefs.GetString("survival_time", "");
        if (!string.IsNullOrEmpty(survivalTime))
        {
            long surTime = long.Parse(survivalTime);
            if (surTime > 0)
            {
                string time = ConvertSecondsToDay(surTime);
                txSurvivalTime.text = string.Format("Last : {0}", time);
            }
        }
        string bestSurvivalTime = PlayerPrefs.GetString("best_survival_time", "");
        if (!string.IsNullOrEmpty(bestSurvivalTime)){
            long bestTime = long.Parse(bestSurvivalTime);
            if (bestTime > 0)
            {
                string time = ConvertSecondsToDay(bestTime);
                txBestTime.text = string.Format("Best : {0}", time);
            }
        }
        gameObject.SetActive(true);
        if (playAnimator != null)
            playAnimator.SetTrigger("scale_0_1");
    }

    private string ConvertSecondsToDay(long seconds)
    {
        long s = seconds;
        string str = s.ToString();
        TimeSpan time = TimeSpan.FromSeconds(s);
        if (s >= 86400)
            str = time.ToString(@"dd\:hh\:mm\:ss");
        else if (s >= 3600)
            str = time.ToString(@"hh\:mm\:ss");
        else if(s >= 60)
            str = time.ToString(@"mm\:ss");
        str = string.Format("{0} secs", str);
        return str;
    }

    public void ClickPlay()
    {
        if (playAnimator != null)
            playAnimator.transform.localScale = Vector3.zero;
        actionPlay?.Invoke();
        gameObject.SetActive(false);
    }
}
