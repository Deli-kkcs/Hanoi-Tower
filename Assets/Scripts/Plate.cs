using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class Plate : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public enum STATE
    {
        idle,
        selected,
        moving,
    }
    public STATE state = STATE.idle;
    public int size;
    public Text text_size;
    private GameObject plateInst;
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.instance.hasExecuted)
            return;
        if (!(transform.GetSiblingIndex() == transform.parent.childCount - 1)) return;
        
        plateInst = Instantiate(this.gameObject);
        plateInst.name = "duoluoluo";
        plateInst.transform.SetParent(transform.parent.parent);
        plateInst.transform.localScale = new Vector3(1, 1, 1);
        plateInst.transform.position = TransScreenPosToWorld(eventData.position);
        //Debug.Log("Begin Drad:"+ plateInst.transform.position);
        //��ԭ��������Ϊ��͸��
        Color color = GetComponent<Image>().color;
        color.a = 0.5f;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a);
    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (GameManager.instance.hasExecuted)
            return;
        if (!(transform.GetSiblingIndex() == transform.parent.childCount-1)) return;
        //Debug.Log("Drad:" + plateInst.transform.position);
        plateInst.transform.position = TransScreenPosToWorld(eventData.position);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.instance.hasExecuted)
            return;
        if (!(transform.GetSiblingIndex() == transform.parent.childCount - 1)) return;
        Collider2D[] cols = Physics2D.OverlapPointAll(TransScreenPosToWorld(eventData.position));
        foreach(Collider2D col in cols)
        {
            if(col.tag == "Column")
            {
                Transform colTransform = col.gameObject.transform;
                //���������������С�ڸ����ӵ�����
                if(colTransform.childCount == 0 || colTransform.GetChild(colTransform.childCount-1).GetComponent<Plate>().size > size)
                {
                    col.gameObject.GetComponent<Column>().PushPlate(size);
                    Destroy(gameObject);
                    transform.parent.GetComponent<Column>().PopPlate();
                }
            }
        }
        Destroy(plateInst);
        //������Ĳ�͸���Ȼ�ԭ
        Color color = GetComponent<Image>().color;
        color.a = 1f;
        GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a);

        Debug.Log(GameManager.instance.CheckComplete());
    }

    static Vector3 TransScreenPosToWorld(Vector3 pos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
        return new Vector3(worldPos.x, worldPos.y, 0);
    }

    
}
