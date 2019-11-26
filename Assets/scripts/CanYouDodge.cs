using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CanYouDodge : MonoBehaviour
{
    [SerializeField]
    private GameObject circlePrefab;
    [SerializeField]
    private Transform circleParent;
    [SerializeField]
    private GameObject[] characters;
    [SerializeField]
    private Transform playerParent;
    [SerializeField]
    private Animator canYouDodgeAnimator;
    private bool isQuit;
    private PlayerDodge player;
    private bool isThrowing;
    private Circle circle;
    [SerializeField]
    private HelpItemManager helpItem;
    [SerializeField]
    private UIStart uiStart;
    [SerializeField]
    private AdsWatcher ads;
    private List<GameObject> lstObs = new List<GameObject>();
    [SerializeField]
    private Text txSurivalTime;
    private long survivalTime;
    private bool isPlayerDead;

    // Start is called before the first frame update
    void Start()
    {
        UIStartPlay();
        if (ads != null)
        {
            ads.actionWatchSuccess = delegate
            {
                LiveAgain();
            };
            ads.actionWatchFailed = delegate { UIStartPlay(); };
        }
    }

    private async void LiveAgain()
    {
        isQuit = false;
        isThrowing = false;
        isPlayerDead = false;
        if (player != null)
            player.LiveAgain();
        if (helpItem != null)
            helpItem.Clear();
        await Task.Delay(1000);
        RunTime(survivalTime);
        ActiveText();
        RandomAttack();
    }

    private async void UIStartPlay()
    {
        await Task.Delay(1000);
        isQuit = false;
        isThrowing = false;
        isPlayerDead = false;
        Clear();
        if (uiStart != null)
        {
            uiStart.actionPlay = delegate
            {
                CreatePlayer();
            };
            uiStart.Run();
        }
    }

    private void Clear()
    {
        if (lstObs == null || lstObs.Count == 0) return;
        for (int i = 0; i < lstObs.Count; i++)
        {
            if (lstObs[i] != null)
                Destroy(lstObs[i]);
        }
        if (helpItem != null)
            helpItem.Clear();
        SetTxTime(0);
    }

    private void CreatePlayer()
    {
        int indexPlayer = PlayerPrefs.GetInt("index_player", 0);
        indexPlayer = Random.Range(0, characters.Length);
        if (characters == null || characters.Length <= indexPlayer) return;
        GameObject ob = Instantiate(characters[indexPlayer]);
        ob.SetActive(false);
        ob.transform.SetParent(playerParent);
        ob.transform.localScale = new Vector3(50, 50, 50);
        RectTransform rectPlayer = ob.GetComponent<RectTransform>();
        if (rectPlayer != null)
        {
            rectPlayer.anchorMin = rectPlayer.anchorMax = new Vector2(0.5f, 0);
            rectPlayer.anchoredPosition = new Vector3(-(Screen.width / 2 + 100), 130.9f, 0);
        }
        ob.SetActive(true);
        lstObs.Add(ob);
        player = ob.GetComponent<PlayerDodge>();
        if (player != null)
        {
            player.MovetoCenter(delegate
            {
                GameObject circleOb = Instantiate(circlePrefab);
                circleOb.SetActive(false);
                circleOb.transform.SetParent(circleParent);
                circleOb.transform.localScale = Vector3.one;
                circle = circleOb.GetComponent<Circle>();
                circle.actionPlayerDead = delegate
                {
                    isPlayerDead = true;
                    PlayerDead(circle);
                };
                lstObs.Add(circleOb);
                circle.Run(delegate { RunTime(-1); ActiveText(); RandomAttack(); });
            });
        }
    }

    private async void RunTime(long time)
    {
        survivalTime = time;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                if (isQuit || isPlayerDead) break;
                survivalTime++;
                SetTxTime(survivalTime);
            }
            await Task.Delay(1000);
        }
        PlayerPrefs.SetString("survival_time", survivalTime.ToString());
        string bestSurvivalTime = PlayerPrefs.GetString("best_survival_time", "");
        if (!string.IsNullOrEmpty(bestSurvivalTime))
        {
            long bestTime = long.Parse(bestSurvivalTime);
            if (survivalTime > bestTime)
                PlayerPrefs.SetString("best_survival_time", survivalTime.ToString());
        }
        else
            PlayerPrefs.SetString("best_survival_time", survivalTime.ToString());
        PlayerPrefs.Save();

    }

    private void SetTxTime(long time)
    {
        if (txSurivalTime != null)
            txSurivalTime.text = time == 0 ? "" : time.ToString();
    }

    private async void PlayerDead(Circle circle)
    {
        if (player != null)
        {
            player.Dead(delegate
            {

            });
        }
        await Task.Delay(500);
        if (circle != null)
        {
            AudioClip giggles = Resources.Load<AudioClip>("sounds/cute_giggles");
            if (SoundManager.instance != null)
                SoundManager.instance.Play(giggles);
            circle.CreateMemeFaceWin(delegate
            {
                int totalGamePlayed = PlayerPrefs.GetInt("game_played", 8);
                if (totalGamePlayed >= 8)
                    AskWatchAds();
                else
                {
                    totalGamePlayed++;
                    PlayerPrefs.SetInt("game_played", totalGamePlayed);
                    PlayerPrefs.Save();
                    UIStartPlay();
                }
            });
        }
    }

    private void AskWatchAds()
    {
        if (ads != null)
            ads.Run();
    }

    private async void ActiveText()
    {
        if (canYouDodgeAnimator != null)
        {
            canYouDodgeAnimator.transform.localScale = Vector3.zero;
            canYouDodgeAnimator.gameObject.SetActive(true);
            canYouDodgeAnimator.SetTrigger("scale_0_11_1");
            await Task.Delay(2000);
            canYouDodgeAnimator.gameObject.SetActive(false);
        }
    }

    private async void RandomAttack()
    {
        if (circle == null) return;
        await Task.Delay(500);
        int rd = Random.Range(0, 2);
        if (rd == 1)
        {
            circle.Throw(player, 2000, delegate
            {
                RandomAttack();
            });
            circle.actionCreateHelpItem = (ob, rd_item) => { CreateHelpItem(ob, rd_item); };
        }
        else
            RandomAttack();
    }

    private async void CreateHelpItem(GameObject ob, int rd_item)
    {
        if (helpItem == null) return;
        helpItem.actionSuccess = delegate
        {
            if (isPlayerDead) return;
            if (circle != null)
            {
                circle.DestroyRectItem(ob, rd_item, 0);
                int rd = Random.Range(0, 2);
                if (rd == 0)
                    circle.CreateMemeFaceLost();
            }
            RandomAttack();
        };
        await Task.Delay(1000);
        helpItem.Run();
    }
}
