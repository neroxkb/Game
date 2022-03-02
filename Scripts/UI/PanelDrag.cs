using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isPrecision = true;  //精准拖拽为true ,鼠标一直在UI中心可以为false
    //存储图片中心点与鼠标点击点的偏移量
    private Vector3 offect;
    //存储当前拖拽图片的RectTransform组件
    private RectTransform m_rt;
    void Start()
    {
        m_rt = this.transform.GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //如果是精确拖拽则进行计算偏移量操作
        if (isPrecision)
        {
            // 存储点击时的鼠标坐标
            Vector3 tWorldPos;
            //UI屏幕坐标转换为世界坐标
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out tWorldPos);
            //计算偏移量   
            offect = transform.position - tWorldPos;

        }
        //否则，默认偏移量为0
        else
        {
            offect = Vector3.zero;
        }
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);

    }

    //拖拽过程中触发
    public void OnDrag(PointerEventData eventData)
    {
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);
    }

    //结束拖拽触发
    public void OnEndDrag(PointerEventData eventData)
    {
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);
    }
    private void SetDraggedPosition(PointerEventData eventData)
    {
        //存储当前鼠标所在位置
        Vector3 globalMousePos;
        //UI屏幕坐标转换为世界坐标
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            //设置位置及偏移量
            m_rt.position = globalMousePos + offect;
        }
    }

}


