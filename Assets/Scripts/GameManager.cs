using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    

    
    private List<GameObject> list_columns = new();
    public GameObject prefab_column;
    public GameObject panel_column;
    [Header("柱子参数")]
    public float intervalColumn = 300f;
    public float heightColumn = 750f;
    public float widthColumn = 30f;

    [Header("盘子参数")]
    public int countPlate = 3;
    [HideInInspector]
    public float heightPlate;
    [HideInInspector]
    public float maxWidthPlate;
    [HideInInspector]
    public float minWidthPlate;

    [Header("等待递归函数执行结束的CD")]
    public float timeWaitForExecute = 1f;
    [Header("等待递归函数执行结束的计时器,每次移动时刷新计时")]
    public float timerWaitForExecute;
    [Header("每次移动的时间间隔")]
    public float intervalMove = 0.3f;
    public int maxCountPlate = 20;
    private bool hasExecuted = false;
    public class MoveInfo
    {
        public int source;
        public int destination;
        public MoveInfo(int s,int d)
        {
            source = s;
            destination = d;
        }
    }
    [SerializeField]
    List<MoveInfo> list_moveInfos = new();
    private void Awake()
    {
        instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
            GenerateColumn();

        if (Input.GetKeyDown(KeyCode.R) && !hasExecuted)
            CallExecute();
        
        
    }

    public void GenerateColumn()
    {
        if (countPlate > maxCountPlate) return;

        heightPlate = heightColumn / (countPlate + 1.5f);
        maxWidthPlate = intervalColumn * 0.8f;
        minWidthPlate = intervalColumn * 0.2f;

        list_moveInfos.Clear();
        list_columns.Clear();
        ClearChild(panel_column);
        float x0 = intervalColumn * -0.5f;
        for(int i=0;i<3;i++)
        {
            GameObject t_column = Instantiate(prefab_column,panel_column.transform);
            t_column.GetComponent<RectTransform>().localPosition = new Vector2(x0 + (i+1)* intervalColumn, heightColumn/2);
            t_column.GetComponent<RectTransform>().sizeDelta = new Vector2(widthColumn, heightColumn);
            t_column.SetActive(true);
            list_columns.Add(t_column);
        }
        list_columns[0].GetComponent<Column>().GeneratePlate();

        hasExecuted = false;
    }
    void move(int x, int y)
    {
        timerWaitForExecute = timeWaitForExecute;
        list_moveInfos.Add(new(x, y));
        //printf("%c->%c\n", x, y);
    }
    void CallExecute()
    {
        if (countPlate >= maxCountPlate)
            return;
        hasExecuted = true;

        list_moveInfos.Clear();
        timerWaitForExecute = timeWaitForExecute;
        WaitForExecute();
        Execute(countPlate, 0, 1, 2);
    }
    void Execute(int n, int a, int b, int c)
    {
        if (n == 1)
        {
            move(a, c);
        }
        else
        {
            Execute(n - 1, a, c, b);//将A座上的n-1个盘子借助C座移向B座
            move(a, c);             //将A座上最后一个盘子移向C座
            Execute(n - 1, b, a, c);//将B座上的n-1个盘子借助A座移向C座
        }
    }

    void WaitForExecute()
    {
        if(timerWaitForExecute >= 0f)
        {
            timerWaitForExecute -= 0.02f;
            Invoke(nameof(WaitForExecute),0.02f);
            return;
        }
        Debug.Log("End WaitForExecute()");
        AutoMove();
    }
    void AutoMove()
    {
        if(list_moveInfos.Count > 0)
        {
            MoveInfo t_info = list_moveInfos[0];
            //Debug.Log("move " + t_info.source + " -> " + t_info.destination);
            list_columns[t_info.destination].GetComponent<Column>().PushPlate(list_columns[t_info.source].GetComponent<Column>().PopPlate());
            list_moveInfos.Remove(t_info);
            Invoke(nameof(AutoMove), intervalMove);
            return;
        }
    }
    public void ClearChild(GameObject p)
    {
        for (int i = 0; i < p.transform.childCount; i++)
            if(p.transform.GetChild(i).gameObject.activeSelf)
                Destroy(p.transform.GetChild(i).gameObject);
    }

    void OnValidate()
    {
        if(Application.isPlaying && instance)
            GenerateColumn();
    }
}
