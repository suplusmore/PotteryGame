using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{

    public SkinnedMeshRenderer skinedRenderer;
    public int blendShapeIndex;

    private Vector3 mOffset;
    private float mZCoord;

    void OnMouseDown()
    {

        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        float x = Vector3.Distance(GetMouseAsWorldPoint(), transform.position);
        float y = Vector3.Distance(mOffset, transform.position);

        float w = skinedRenderer.GetBlendShapeWeight(blendShapeIndex);

        if (x > y)
        {
            skinedRenderer.SetBlendShapeWeight(blendShapeIndex, Mathf.Clamp(w - 1f, 0, 70));
            skinedRenderer.SetBlendShapeWeight(blendShapeIndex - 1, Mathf.Clamp(w - 0.5f, 0, 70));
            skinedRenderer.SetBlendShapeWeight(blendShapeIndex + 1, Mathf.Clamp(w - 0.5f, 0, 70));
        }
        else if (x < y)
        {
            skinedRenderer.SetBlendShapeWeight(blendShapeIndex, Mathf.Clamp(w + 1f, 0, 70));
            skinedRenderer.SetBlendShapeWeight(blendShapeIndex - 1, Mathf.Clamp(w + 0.5f, 0, 70));
            skinedRenderer.SetBlendShapeWeight(blendShapeIndex + 1, Mathf.Clamp(w + 0.5f, 0, 70));
        }
    }
}