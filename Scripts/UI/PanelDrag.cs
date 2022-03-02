using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isPrecision = true;  //��׼��קΪtrue ,���һֱ��UI���Ŀ���Ϊfalse
    //�洢ͼƬ���ĵ�����������ƫ����
    private Vector3 offect;
    //�洢��ǰ��קͼƬ��RectTransform���
    private RectTransform m_rt;
    void Start()
    {
        m_rt = this.transform.GetComponent<RectTransform>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //����Ǿ�ȷ��ק����м���ƫ��������
        if (isPrecision)
        {
            // �洢���ʱ���������
            Vector3 tWorldPos;
            //UI��Ļ����ת��Ϊ��������
            RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out tWorldPos);
            //����ƫ����   
            offect = transform.position - tWorldPos;

        }
        //����Ĭ��ƫ����Ϊ0
        else
        {
            offect = Vector3.zero;
        }
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);

    }

    //��ק�����д���
    public void OnDrag(PointerEventData eventData)
    {
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);
    }

    //������ק����
    public void OnEndDrag(PointerEventData eventData)
    {
        //m_rt.position = Input.mousePosition + offect;
        SetDraggedPosition(eventData);
    }
    private void SetDraggedPosition(PointerEventData eventData)
    {
        //�洢��ǰ�������λ��
        Vector3 globalMousePos;
        //UI��Ļ����ת��Ϊ��������
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_rt, eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            //����λ�ü�ƫ����
            m_rt.position = globalMousePos + offect;
        }
    }

}


