using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDodge : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public float speed;
    private float currentMouseX;
    private float currentMouseY;
    private bool isReady;
    private float halfScreenWidth;
    private bool blockTrigger;
    private int isRunningRight;
    private string currentStateAnimator;
    private bool blockJump;
    private bool isDead;
    [SerializeField]
    private Rigidbody2D rigidPlayer;
    private AudioClip jumpAudio;
    private AudioClip runAudio;

    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        halfScreenWidth = Screen.width / 2;
        blockTrigger = false;
        blockJump = false;
        isRunningRight = -1;
        isDead = false;
    }

    public async void MovetoCenter(Action action)
    {
        isReady = false;
        if(runAudio == null)
            runAudio = Resources.Load<AudioClip>("sounds/footsteps_dry_leave");
        if (SoundManager.instance != null)
            SoundManager.instance.Play(runAudio);
        currentStateAnimator = "run";
        SetTrigger(currentStateAnimator);
        speed = 200;
        Vector3 current = rect.anchoredPosition;
        Vector3 target = new Vector3(0, 130.9f, 0);
        while (true)
        {
            if (Time.timeScale == 1)
            {
                current = Vector3.MoveTowards(current, target, speed * Time.deltaTime);
                rect.anchoredPosition = current;
                if ((current - target).sqrMagnitude < (0.1 * 0.1))
                    break;
            }
            await Task.Delay(1);
        }
        speed = 260;
        if (SoundManager.instance != null)
            SoundManager.instance.Stop();
        currentStateAnimator = "idle";
        SetTrigger(currentStateAnimator);
        action?.Invoke();
        isReady = true;
    }

    public void SetTrigger(string trigger)
    {
        if (animator != null)
            animator.SetTrigger(trigger);
    }

    private void Update()
    {
        if (isReady == false || Time.timeScale == 0 || isDead == true) return;
        if (Input.GetMouseButtonDown(0))
        {
            currentMouseX = Input.mousePosition.x;
            currentMouseY = Input.mousePosition.y;
        }
        if (Input.GetMouseButton(0))
        {
            float targetX = Input.mousePosition.x;
            float targetY = Input.mousePosition.y;
            Vector3 currentPosPlayer = rect.anchoredPosition;
            if (targetY > (currentMouseY + 100) && blockJump == false)
            {
                blockJump = true;
                Jump();
            }
            if (targetX > currentMouseX || (isRunningRight == 1 && targetX >= currentMouseX))
            {
                if (isRunningRight != 1)
                {
                    spriteRenderer.flipX = false;
                    isRunningRight = 1;
                    if (SoundManager.instance != null)
                    {
                        if (runAudio == null)
                            runAudio = Resources.Load<AudioClip>("sounds/footsteps_dry_leave");
                        SoundManager.instance.Stop();
                        SoundManager.instance.Play(runAudio);
                    }
                    currentStateAnimator = "run";
                    SetTrigger(currentStateAnimator);
                }
                float newX = currentPosPlayer.x + (speed * Time.deltaTime);
                if (newX > (halfScreenWidth - 50))
                    newX = halfScreenWidth - 50;
                currentPosPlayer = new Vector3(newX, currentPosPlayer.y, 0);
                rect.anchoredPosition = currentPosPlayer;
                currentMouseX = targetX;

            }
            else if (targetX < currentMouseX || (isRunningRight == 0 && targetX <= currentMouseX))
            {
                if (isRunningRight != 0)
                {
                    spriteRenderer.flipX = true;
                    isRunningRight = 0;
                    if (SoundManager.instance != null)
                    {
                        if (runAudio == null)
                            runAudio = Resources.Load<AudioClip>("sounds/footsteps_dry_leave");
                        SoundManager.instance.Stop();
                        SoundManager.instance.Play(runAudio);
                    }
                    currentStateAnimator = "run";
                    SetTrigger(currentStateAnimator);
                }
                float newX = currentPosPlayer.x - (speed * Time.deltaTime);
                if (newX < (-(halfScreenWidth - 50)))
                    newX = -(halfScreenWidth - 50);
                currentPosPlayer = new Vector3(newX, currentPosPlayer.y, 0);
                rect.anchoredPosition = currentPosPlayer;
                currentMouseX = targetX;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            blockTrigger = false;
            isRunningRight = -1;
            if (blockJump == false)
            {
                if (SoundManager.instance != null)
                    SoundManager.instance.Stop();
                currentStateAnimator = "idle";
                SetTrigger(currentStateAnimator);
            }
        }
    }

    public async void Dead(Action action)
    {
        isDead = true;
        AudioClip dead = Resources.Load<AudioClip>("sounds/dead");
        if (SoundManager.instance != null)
        {
            SoundManager.instance.Stop();
            SoundManager.instance.Play(dead);
        }
        if (animator != null)
            animator.SetTrigger("dead");
        await Task.Delay(2000);
        action?.Invoke();
    }

    public void LiveAgain()
    {
        Reset();
        if (animator != null)
            animator.SetTrigger("idle");
    }

    private async void Jump()
    {
        if (isDead || Time.timeScale == 0) return;
        if (jumpAudio == null)
            jumpAudio = Resources.Load<AudioClip>("sounds/jump");
        if (SoundManager.instance != null)
            SoundManager.instance.Play(jumpAudio);
        rigidPlayer.AddForce(new Vector2(0, 4), ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (blockTrigger == true || isDead || isReady == false) return;
        if (collision == null || !collision.collider.CompareTag("plane")) return;
        blockTrigger = true;
        if (isRunningRight == -1)
            currentStateAnimator = "idle";
        SetTrigger(currentStateAnimator);
        blockJump = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isRunningRight == -1 || blockTrigger == false || isDead || isReady == false) return;
        if (collision == null || !collision.collider.CompareTag("plane")) return;
        blockTrigger = false;
        currentStateAnimator = "jump";
        SetTrigger(currentStateAnimator);
    }

}
