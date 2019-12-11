using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericKeyboardMouse : MonoBehaviour
{
    public ParticleSystem key_highlight;
    public InputField unmapped_key_list;
    public Text m_mouseInputPosText;
    public Text m_mouseEventPosText;

    // Update is called once per frame
    void Update()
    {
        // Keyboard input or mouse button is pressed
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                StartHighlightKey(kcode.ToString());

            if (Input.GetKeyUp(kcode))
                StopHighlightKey(kcode.ToString());
        }

        // Mouse move
        float moveX = Input.GetAxis("Mouse X");
        if (Mathf.Abs(moveX) > 0.5)
        {
            if (moveX > 0)
            {
                StartHighlightMouse("Move_Right");
                StopHighlightMouse("Move_Left");
            }
            else
            {
                StartHighlightMouse("Move_Left");
                StopHighlightMouse("Move_Right");
            }
        }
        else
        {
            StopHighlightMouse("Move_Left");
            StopHighlightMouse("Move_Right");
        }

        float moveY = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(moveY) > 0.5)
        {
            if (moveY > 0)
            {
                StartHighlightMouse("Move_Up");
                StopHighlightMouse("Move_Down");
            }
            else
            {
                StartHighlightMouse("Move_Down");
                StopHighlightMouse("Move_Up");
            }
        }
        else
        {
            StopHighlightMouse("Move_Up");
            StopHighlightMouse("Move_Down");
        }

        // Mouse wheel
        float wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0)
        {
            StartHighlightMouse("Wheel_Up");
            StopHighlightMouse("Wheel_Down");
        }
        else if (wheel < 0)
        {
            StartHighlightMouse("Wheel_Down");
            StopHighlightMouse("Wheel_Up");
        }
        else
        {
            StopHighlightMouse("Wheel_Up");
            StopHighlightMouse("Wheel_Down");
        }

        // Show mouse position
        if (m_mouseInputPosText != null)
            m_mouseInputPosText.text = Input.mousePosition.ToString("F0");
    }

    void OnGUI()
    {
        if (m_mouseEventPosText != null)
            m_mouseEventPosText.text = Event.current.mousePosition.ToString("F0");
    }

    // Generate the blue ring Particle System over the key or mouse button
    private void StartHighlightKey(string keyName)
    {
        Transform key = transform.Find("Keys/" + keyName);
        Debug.Log(keyName + " Down");

        if (key == null)
            AddUnmappedKey(keyName);
        else
        {
            ParticleSystem ps = key.GetComponentInChildren<ParticleSystem>();
            if (ps == null)
                Instantiate(key_highlight, key.transform.position, key.transform.rotation, key);
            else
                ps.Play();
        }
    }

    // Stop the Particle System for keys and mouse buttons
    private void StopHighlightKey(string keyName)
    {
        Transform key = transform.Find("Keys/" + keyName);
        Debug.Log(keyName + " Up");

        if (key == null)
            Debug.Log("Cannot Find " + keyName);
        else
        {
            ParticleSystem[] ps = key.GetComponentsInChildren<ParticleSystem>();
            if (ps.Length > 0)
            {
                foreach (ParticleSystem p in ps)
                    p.Stop();
            }
        }
    }

    // Generate the blue arrow for move movement and wheel
    private void StartHighlightMouse(string mouseAction)
    {
        Transform mAction = transform.Find("Mouse/" + mouseAction);

        if (mAction != null)
            mAction.GetComponent<ArrowHighlight>().Play();
    }

    // Stop the arrow highlight
    private void StopHighlightMouse(string mouseAction)
    {
        Transform mAction = transform.Find("Mouse/" + mouseAction);

        if (mAction != null)
            mAction.GetComponent<ArrowHighlight>().Stop();
    }

    // Show the unmapped key name in the text field
    private void AddUnmappedKey(string keyName)
    {
        unmapped_key_list.text += keyName;
        unmapped_key_list.text += "\n";
    }
}
