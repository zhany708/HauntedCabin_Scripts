using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Introduction：
 * Creator：Zhang Yu
*/

public class Canvas : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        await UIManager.Instance.OpenPanel(UIManager.Instance.UIKeys.PlayerStatusBarKey);
    }
}
