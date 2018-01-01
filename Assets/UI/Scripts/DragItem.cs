using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IPointerEnterHandler, IPointerExitHandler
{
	public bool dragOnSurfaces = true;
	
	private Dictionary<int,GameObject> m_DraggingIcons = new Dictionary<int, GameObject>();
	private Dictionary<int, RectTransform> m_DraggingPlanes = new Dictionary<int, RectTransform>();

    private Color normalColor;
    public Color highlightColor = Color.yellow;

    private Sprite nullItem;

    private Image image;

    void Start() {
        nullItem = Resources.Load("Textures/bag_item_null",typeof(Sprite)) as Sprite;
        image = GetComponent<Image>();
        if (image.sprite == nullItem) {
            GetComponent<DragItem>().enabled = false;
        }
    }

    //开始拖动时
    public void OnBeginDrag(PointerEventData eventData)
	{
		var canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

        m_DraggingIcons[eventData.pointerId] = new GameObject("Temp_Item_Swap");

        m_DraggingIcons[eventData.pointerId].transform.SetParent(canvas.transform, false);
        m_DraggingIcons[eventData.pointerId].transform.SetAsLastSibling();

        var image = m_DraggingIcons[eventData.pointerId].AddComponent<Image>();
        
        var group = m_DraggingIcons[eventData.pointerId].AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;

        image.sprite = GetComponent<Image>().sprite;
        GetComponent<Image>().sprite = nullItem;

        if (dragOnSurfaces)
			m_DraggingPlanes[eventData.pointerId] = transform as RectTransform;
		else
			m_DraggingPlanes[eventData.pointerId]  = canvas.transform as RectTransform;
		
		SetDraggedPosition(eventData);
	}

    //拖动发生时一直同步位置和旋转
	public void OnDrag(PointerEventData eventData)
	{
		if (m_DraggingIcons[eventData.pointerId] != null)
			SetDraggedPosition(eventData);
	}

    //设置被拖动图片的位置和旋转
	private void SetDraggedPosition(PointerEventData eventData)
	{
		if (dragOnSurfaces && eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
			m_DraggingPlanes[eventData.pointerId] = eventData.pointerEnter.transform as RectTransform;
		
		var rt = m_DraggingIcons[eventData.pointerId].GetComponent<RectTransform>();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlanes[eventData.pointerId], eventData.position, eventData.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlanes[eventData.pointerId].rotation;
		}
	}

    //结束拖动时
	public void OnEndDrag(PointerEventData eventData)
	{
        GameObject dropObj = eventData.pointerEnter;

        if (dropObj == null) {

            DestroyItem(eventData);
            return;
        }
        else if (dropObj.name != "Drag_Item") {

            GetComponent<Image>().sprite = m_DraggingIcons[eventData.pointerId].GetComponent<Image>().sprite;

            DestroyItem(eventData);

            return;
        }

        Sprite dropSprite = dropObj.GetComponent<Image>().sprite;

        if (dropSprite != null)
        {
            GetComponent<Image>().sprite = dropSprite;
        }

        dropObj.transform.parent.GetComponent<Image>().color = normalColor;

        dropObj.GetComponent<Image>().sprite = m_DraggingIcons[eventData.pointerId].GetComponent<Image>().sprite;
        if (dropObj.GetComponent<Image>().sprite != nullItem) {
            dropObj.GetComponent<DragItem>().enabled = true;
        }

        DestroyItem(eventData);

        if (image.sprite == nullItem) {
            GetComponent<DragItem>().enabled = false;
        }
    }

    //找到对象带有Canvas组件的父对象
	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;
		
		var t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}

    //当物体处于激活状态时
    public void OnEnable()
    {
        Image containerImage = transform.parent.GetComponent<Image>();
        if (containerImage != null)
        {
            normalColor = containerImage.color;
        }
    }

    //当指针进入时
    public void OnPointerEnter(PointerEventData data)
    {
        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
            transform.parent.GetComponent<Image>().color = highlightColor;
    }

    //当指针移出时
    public void OnPointerExit(PointerEventData data)
    {
        transform.parent.GetComponent<Image>().color = normalColor;
    }

    //获取拖动的图片
    private Sprite GetDropSprite(PointerEventData data)
    {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return null;

        var srcImage = originalObj.GetComponent<Image>();
        if (srcImage == null)
            return null;

        return srcImage.sprite;
    }

    void DestroyItem(PointerEventData eventData) {
        if (m_DraggingIcons[eventData.pointerId] != null)
            Destroy(m_DraggingIcons[eventData.pointerId]);

        m_DraggingIcons[eventData.pointerId] = null;
    }
}
