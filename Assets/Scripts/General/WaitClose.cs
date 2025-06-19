using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitClose : MonoBehaviour
{
   public GameObject CloseSomething;

    public void WaitCloseFunction()
    {
        StartCoroutine(ButtonDelayOpen());
    }

    IEnumerator ButtonDelayOpen()
    {
        Debug.Log(Time.time);
        yield return new WaitForSeconds(5f);
        Debug.Log(Time.time);

        // This line will be executed after 2 seconds
        CloseSomething.SetActive(false);
    }
}
