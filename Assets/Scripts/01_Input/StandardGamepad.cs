using UnityEngine;
using UnityEngine.UI;

public class StandardGamepad : MonoBehaviour
{
    public InputField unmapped_button_list;
    public AnalogKey[] analog_keys;
    public AnalogStick[] analog_sticks;

    // Use this for initialization
    void Start()
    {
        Debug.Log(Input.GetJoystickNames().ToString());

        // Assign the original position for all sticks in scene
        foreach (AnalogStick stick in analog_sticks)
            stick.original_position = stick.stick.position;
    }

    // Update is called once per frame
    void Update()
    {
        // When a joystick button is pressed
        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                string buttonName = GetButtonName(kcode);
                if (buttonName != null)
                {
                    Transform button = FindButtonTransform(buttonName);
                    Debug.Log(buttonName + " Down");

                    if (button == null)
                        AddUnmappedButton(buttonName);
                    else
                        button.gameObject.SetActive(true);
                }
            }


            if (Input.GetKeyUp(kcode))
            {
                string buttonName = GetButtonName(kcode);
                if (buttonName != null)
                {
                    Transform button = FindButtonTransform(buttonName);
                    Debug.Log(buttonName + " Up");

                    if (button != null)
                        button.gameObject.SetActive(false);
                }
            }
        }

        // Update all analog keys
        foreach (AnalogKey key in analog_keys)
        {
            float inputValue = Input.GetAxis(key.input_name) * 10;
            if (inputValue > key.trigger_value)
                key.trigger.SetActive(true);
            else
                key.trigger.SetActive(false);
        }

        // Update all analog sticks
        foreach (AnalogStick stick in analog_sticks)
        {
            float inputX = Input.GetAxis(stick.input_x_name) * 10 * stick.move_range;
            float inputY = Input.GetAxis(stick.input_y_name) * -10 * stick.move_range;
            stick.stick.position = stick.original_position + new Vector3(inputX, inputY, 0f);
        }
    }

    // Remove "Joystick" from the key code value to find the button through name
    private string GetButtonName(KeyCode kcode)
    {
        string kcodeString = kcode.ToString();
        if (kcodeString.Contains("JoystickButton"))
            return kcodeString.Replace("Joystick", "");
        else
            return null;
    }

    private Transform FindButtonTransform(string buttonName)
    {
        Transform buttonList = transform.Find("Buttons");
        Transform button = buttonList.Find(buttonName);

        if (button == null)
        {
            foreach (Transform child in buttonList)
            {
                button = child.Find(buttonName);
                if (button != null)
                    break;
            }
        }
        return button;
    }

    private void AddUnmappedButton(string buttonName)
    {
        unmapped_button_list.text += buttonName;
        unmapped_button_list.text += "\n";
    }
}
