using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpItem : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectHelpItem;
    [SerializeField]
    private Animator helpItemAnimator;
    public Action actionMeetPlayer;

    public void SetTrigger(string state)
    {
        if (helpItemAnimator == null) return;
        helpItemAnimator.SetTrigger(state);
    }

    public RectTransform GetRect()
    {
        return rectHelpItem;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || !collision.CompareTag("Player")) return;
        actionMeetPlayer?.Invoke();
        SetActive(false);
    }
}
