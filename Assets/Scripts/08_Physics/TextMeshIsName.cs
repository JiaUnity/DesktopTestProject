using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextMeshIsName : MonoBehaviour
{
    public bool isParentName = false;
    public string prefix = "";
    public string suffix = "";

#if UNITY_EDITOR
    void Update()
    {
        if (isParentName && transform.parent)
            GetComponent<TextMesh>().text = prefix + transform.parent.name + suffix;
        else
            GetComponent<TextMesh>().text = prefix + gameObject.name + suffix;
    }

#endif
}
