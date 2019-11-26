using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private float defaultSpeed;
    public float speed;
    public float speedIncreaseByTime;
    private bool isQuit;

    // Start is called before the first frame update
    void Start()
    {
        isQuit = false;
        defaultSpeed = speed;
        Rotating();
    }

    public float GetDefaultSpeed()
    {
        return defaultSpeed;
    }

    private async void Rotating()
    {
        while (true)
        {
            if (Time.timeScale == 1)
            {
                if (transform == null || isQuit) break;
                transform.Rotate(0, 0, speed * Time.deltaTime);
            }
            await Task.Delay(10);
        }
    }

    private void OnApplicationQuit()
    {
        isQuit = true;
    }
}
