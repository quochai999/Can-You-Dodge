using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MathSaveLife : MonoBehaviour
{
    [SerializeField]
    private Text txQuestion, txTime;
    [SerializeField]
    private Text[] txAnswers;
    private int rdResultPos;
    public Action actionFailed;
    public Action actionSuccess;
    private bool isClickedResult;

    public async void Run()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.Stop();
        int rdNo1 = Random.Range(0, 101);
        int rdNo2 = Random.Range(0, 101);
        int rdOperator = Random.Range(0, 2);
        int result = 0;
        string operate = string.Empty;
        if (rdOperator == 0) {
            result = rdNo1 + rdNo2;
            operate = "+";
        }
        else {
            result = rdNo1 - rdNo2;
            operate = "-";
        }
        if (txQuestion != null)
            txQuestion.text = string.Format("{0} {1} {2} = ?", rdNo1, operate, rdNo2);
        rdResultPos = Random.Range(0, 3);
        txAnswers[rdResultPos].text = result.ToString();
        int reSultMinus1 = result - 1;
        int reSultPlus1 = result + 1;
        int rdAddLst = Random.Range(0, 2);
        int[] arr = new int[2];
        if(rdAddLst == 0)
        {
            arr[0] = reSultMinus1;
            arr[1] = reSultPlus1;
        }
        else
        {
            arr[0] = reSultPlus1;
            arr[1] = reSultMinus1;
        }
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            if (i == rdResultPos) continue;
            txAnswers[i].text = arr[index].ToString();
            index++;
        }
        isClickedResult = false;
        gameObject.SetActive(true);
        Time.timeScale = 0;
        if (txTime == null) return;
        for (int i = 4; i > 0; i--)
        {
            if (isClickedResult) return;
            txTime.text = i.ToString();
            await Task.Delay(1000);
        }
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void ClickResult(int i)
    {
        Time.timeScale = 1;
        isClickedResult = true;
        if (i == rdResultPos)
            actionSuccess?.Invoke();
        else
            actionFailed?.Invoke();
        gameObject.SetActive(false);
    }
}
