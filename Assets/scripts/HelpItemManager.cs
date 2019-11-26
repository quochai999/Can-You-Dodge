using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelpItemManager : MonoBehaviour
{
    [SerializeField]
    private HelpItem helpItem;
    private float halfScreenWidth;
    public Action actionFailed;
    public Action actionSuccess;
    [SerializeField]
    private MathSaveLife math;

    private void Start()
    {
        halfScreenWidth = Screen.width / 2;
        if (helpItem != null)
        {
            helpItem.actionMeetPlayer = delegate
            {
                RunMath();
            };
        }
        if(math != null)
        {
            math.actionSuccess = delegate {
                actionSuccess?.Invoke();
            };
            math.actionFailed = delegate {
                actionFailed?.Invoke();
            };
        }
    }

    public void Clear()
    {
        if (helpItem != null)
            helpItem.SetActive(false);
    }

    public void Run()
    {
        helpItem.SetActive(false);
        float helpItemPos = Random.Range(-(halfScreenWidth - 50), halfScreenWidth - 50);
        helpItem.GetRect().anchoredPosition = new Vector2(helpItemPos, 0);
        helpItem.SetActive(true);
        helpItem.SetTrigger("scale_11_1");
    }

    private void RunMath()
    {
        if (math == null) return;
        math.Run();
    }
}
