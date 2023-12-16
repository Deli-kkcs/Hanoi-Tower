using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public Text plateNum;
    public void UpdatePlate()
    {
        int plateCnt;
        bool isNum  = int.TryParse(plateNum.text,out plateCnt);
        if (isNum)
        {
            GameManager.instance.countPlate = plateCnt;
            GameManager.instance.GenerateColumn();
        }
    }
}
