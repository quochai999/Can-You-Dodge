using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public Action actionMeetPlayer;
    public Action actionMiss;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Player"))
        {
            actionMeetPlayer?.Invoke();
        }
        else if (collision.CompareTag("better_luck_next_time"))
        {
            actionMiss?.Invoke();
        }
    }
}
