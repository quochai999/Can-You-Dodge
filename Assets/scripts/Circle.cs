using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum AttackState
{
    PhiMotNua = 0,
    PhiCaQuangDuong,
    PhiDuoi
}

public enum MotNuaState
{
    ThuVe = 0,
    PhiCaiKhac
}

public class Circle : MonoBehaviour
{
    [SerializeField]
    private Image[] imgs;
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private Image memeImg;
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private Transform circle;
    [SerializeField]
    private Animator memeAnimator;
    private bool isQuit;
    public float speed;
    public float speedPhiDaoLocal;
    public float speedPhiDaoPosition;
    public float speedPhiDaoDuoi;
    public float speedThuVe;
    private PlayerDodge player;
    private bool isStopLookAt;
    public Action<GameObject, int> actionCreateHelpItem;
    public Action actionPlayerDead;


    public void Run(Action action)
    {
        rect.anchoredPosition = new Vector3(-(Screen.width / 2 + 100), 400, 0);
        CreateItems();
        CreateMemeFaceWin(null);
        gameObject.SetActive(true);
        MovetoCenter(action);
    }

    private void CreateItems()
    {
        for (int i = 0; i < imgs.Length; i++)
        {
            int rd = UnityEngine.Random.Range(0, sprites.Length);
            Image img = imgs[i];
            img.sprite = sprites[rd];
            img.preserveAspect = true;
        }
    }

    public async void CreateMemeFaceWin(Action action)
    {
        CreateMemeFace("win");
        if (memeAnimator != null)
            memeAnimator.SetTrigger("meme_win");
        await Task.Delay(2000);
        action?.Invoke();
    }

    public void CreateMemeFaceLost()
    {
        CreateMemeFace("lost");
    }

    private void CreateMemeFace(string win_lost)
    {
        int rd = UnityEngine.Random.Range(1, 9);
        Sprite sprite = Resources.Load<Sprite>(string.Format("icons/{0}/{1}_{2}", win_lost, win_lost, rd));
        memeImg.sprite = sprite;
        memeImg.preserveAspect = true;
    }

    private async void MovetoCenter(Action action)
    {
        Vector3 current = rect.anchoredPosition;
        Vector3 target = new Vector3(0, 400, 0);
        float deltaTime = Time.deltaTime;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                current = Vector3.MoveTowards(current, target, speed * Time.deltaTime);
                if (rect == null) break;
                rect.anchoredPosition = current;
                if ((current - target).sqrMagnitude < (0.1 * 0.1))
                    break;
            }
            await Task.Delay(10);
        }
        action?.Invoke();
        await Task.Delay(1000);
        speed = 400; RandomMove();
    }

    private async void RandomMove()
    {
        isQuit = false;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                if (isQuit == true || rect == null)
                    break;
                float rd = UnityEngine.Random.Range(-300, 300);
                Vector3 current = rect.anchoredPosition;
                Vector3 target = new Vector3(rd, 400, 0);
                while (true)
                {
                    if (Time.timeScale == 1)
                    {
                        current = Vector3.MoveTowards(current, target, speed * Time.deltaTime);
                        if (rect == null) break;
                        rect.anchoredPosition = current;
                        if ((current - target).sqrMagnitude < (0.1 * 0.1) || isQuit == true)
                            break;
                    }
                    await Task.Delay(10);
                }
            }
            await Task.Delay(100);
        }
    }

    public async void Throw(PlayerDodge player, int wait_time, Action action)
    {
        if (player == null) return;
        this.player = player;
        int rdItem = Random.Range(0, imgs.Length);
        isStopLookAt = false;
        RectTransform rectItem = CreateItem(rdItem, action);
        LookAt(rectItem, player);
        await Task.Delay(wait_time);
        RectTransform rectPlayer = player.GetComponent<RectTransform>();

        int rdPhiState = Random.Range(0, 3);
        if (rdPhiState == (int)AttackState.PhiMotNua)
        {
            isStopLookAt = true;
            PhiMotNua(rectItem, rectPlayer, rdItem, action);
        }
        else if (rdPhiState == (int)AttackState.PhiCaQuangDuong)
        {
            isStopLookAt = true;
            PhiCaQuangDuong(rectItem, rectPlayer, rdItem, delegate
            {
                DestroyRectItem(null, rdItem, 400);
                action?.Invoke();
            });
        }
        else
        {
            actionCreateHelpItem?.Invoke(rectItem.gameObject, rdItem);
            PhiDuoi(rectItem, rectPlayer, rdItem, action);
        }
    }

    public async void DestroyRectItem(GameObject ob, int rd_item, int wait_time)
    {
        await Task.Delay(wait_time);
        if (ob != null)
            Destroy(ob);
        if (imgs != null && imgs.Length > rd_item)
            imgs[rd_item].gameObject.SetActive(true);
    }

    private RectTransform CreateItem(int rd_item, Action action_miss)
    {
        GameObject prefab = imgs[rd_item].gameObject;
        prefab.SetActive(false);
        GameObject itemOb = Instantiate(prefab);
        itemOb.transform.SetParent(circle);
        itemOb.transform.localScale = Vector3.one;
        itemOb.transform.localPosition = prefab.transform.localPosition;
        Canvas canvasItem = itemOb.GetComponent<Canvas>();
        canvasItem.sortingOrder = 2;
        Item item = itemOb.GetComponent<Item>();
        if (item != null)
        {
            item.actionMeetPlayer = delegate
            {
                DestroyRectItem(itemOb, rd_item, 0);
                actionPlayerDead?.Invoke();
            };
            item.actionMiss = delegate
            {
                int rdMemeFaceChance = Random.Range(0, 2);
                if (rdMemeFaceChance == 0)
                    CreateMemeFaceLost();
                DestroyRectItem(itemOb, rd_item, 0);
                action_miss?.Invoke();
            };
        }
        itemOb.SetActive(true);
        RectTransform rectItem = itemOb.GetComponent<RectTransform>();
        return rectItem;
    }

    private async void PhiMotNua(RectTransform rect_item, RectTransform rect_player, int rd_item, Action action)
    {
        rect_item.SetParent(rect_player.transform);
        Vector2 current = rect_item.anchoredPosition;
        Vector2 target = rect_item.anchoredPosition * 0.3f;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                current = Vector2.MoveTowards(current, target, speedPhiDaoLocal * Time.deltaTime);
                if (rect_item == null) break;
                rect_item.anchoredPosition = current;
                if ((current - target).sqrMagnitude < 0.1f * 0.1f || isQuit)
                    break;
            }
            await Task.Delay(1);
        }
        rect_item.SetParent(transform.parent.transform);
        int rdPhiState = Random.Range(0, 2);
        if (rdPhiState == (int)MotNuaState.ThuVe)
        {
            await Task.Delay(1000);
            Destroy(rect_item.gameObject);
            action?.Invoke();
            await Task.Delay(200);
            imgs[rd_item].gameObject.SetActive(true);
        }
        else
        {
            Throw(player, 500, action);
            await Task.Delay(400);
            Destroy(rect_item.gameObject);
            await Task.Delay(200);
            imgs[rd_item].gameObject.SetActive(true);
        }
    }

    private async void PhiCaQuangDuong(RectTransform rect_item, RectTransform rect_player, int rd_item, Action action)
    {
        rect_item.SetParent(transform.parent.transform);
        Vector2 current = rect_item.position;
        Vector2 target = rect_player.position + (rect_player.position * 0.7f);
        while (true)
        {
            if (Time.timeScale == 1)
            {
                current = Vector2.MoveTowards(current, target, speedPhiDaoPosition * Time.deltaTime);
                if (rect_item == null) break;
                rect_item.position = current;
                if ((current - target).sqrMagnitude < 0.1f * 0.1f || isQuit)
                    break;
            }
            await Task.Delay(1);
        }
    }

    private async void PhiDuoi(RectTransform rect_item, RectTransform rect_player, int rd_item, Action action)
    {
        rect_item.SetParent(transform.parent.transform);
        Vector2 current = rect_item.position;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                if (rect_player == null) break;
                Vector2 target = rect_player.position;
                current = Vector2.MoveTowards(current, target, speedPhiDaoDuoi * Time.deltaTime);
                if (rect_item == null) break;
                rect_item.position = current;
                if ((current - target).sqrMagnitude < 0.1f * 0.1f || isQuit)
                    break;
            }
            await Task.Delay(1);
        }
        if (rect_item != null)
        {
            isStopLookAt = true;
            Destroy(rect_item.gameObject);
            actionPlayerDead?.Invoke();
        }
    }


    private async void LookAt(Transform item, PlayerDodge player)
    {
        while (true)
        {
            if (Time.timeScale == 1)
            {
                if (isQuit || isStopLookAt) break;
                if (item == null || player == null) break;
                Vector3 diff = player.transform.position - item.position;
                diff.Normalize();
                float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                if (item == null) break;
                item.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            }
            await Task.Delay(1);
        }
    }

    private void OnApplicationQuit()
    {
        isQuit = true;
    }
}
