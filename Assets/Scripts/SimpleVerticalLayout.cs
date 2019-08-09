using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleVerticalLayout : MonoBehaviour
{
    public int padding = 25;
    [SerializeField]
    public RectTransform[] elements;

    // Update is called once per frame
    void Update()
    {
        if (elements.Length > 1)
        {
            float posY = elements[0].localPosition.y;

            for (int i = 0; i < elements.Length; i++)
            {
                if (i > 0)
                {
                    Vector3 pos = elements[i].localPosition;
                    pos.y = posY;
                    elements[i].localPosition = pos;
                }
                posY -= elements[i].rect.height;
                posY -= padding;
            }
        }
    }
}
