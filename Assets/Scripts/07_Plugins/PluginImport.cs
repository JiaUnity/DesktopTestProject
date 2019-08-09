using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class PluginImport : MonoBehaviour
{
    //Lets make our calls from the Plugin
    [DllImport("ASimplePlugin")]
    private static extern int PrintANumber();

    [DllImport("ASimplePlugin")]
    private static extern IntPtr PrintHello();

    [DllImport("ASimplePlugin")]
    private static extern int AddTwoIntegers(int i1, int i2);

    [DllImport("ASimplePlugin")]
    private static extern float AddTwoFloats(float f1, float f2);

    [Header("Print Hello/Number")]
    public InputField print_hello_result;
    public InputField print_number_result;

    [Header("Add Two Integers")]
    public InputField[] int_input = new InputField[2];
    public InputField add_int_result;

    [Header("Add Two Floats")]
    public InputField[] float_input = new InputField[2];
    public InputField add_float_result;

    void Start()
    {
        //Debug.Log(PrintANumber());
        //Debug.Log(Marshal.PtrToStringAuto (PrintHello()));
        //Debug.Log(AddTwoIntegers(2,2));
        //Debug.Log(AddTwoFloats(2.5F,4F));

        //result_display.text = "Result from plugins:\n";
        //result_display.text += Marshal.PtrToStringAuto(PrintHello()) + "\n";
        //result_display.text += PrintANumber() + "\n";
        //result_display.text += AddTwoIntegers(2, 2) + "\n";
        //result_display.text += AddTwoFloats(2.5F, 4F);
    }

    public void ShowPrintHello()
    {
        print_hello_result.text = Marshal.PtrToStringAnsi(PrintHello());
    }

    public void ShowPrintANumber()
    {
        print_number_result.text = PrintANumber().ToString();
    }

    public void ShowAddInteger()
    {
        int[] values = new int[2];
        for (int i = 0; i < 2; i++)
        {
            if (!int.TryParse(int_input[i].text, out values[i]))
            {
                int_input[i].image.color = new Color(1f, 0.3f, 0.3f);
                add_int_result.text = String.Empty;
                return;
            }
            else
                int_input[i].image.color = Color.white;
        }
        add_int_result.text = AddTwoIntegers(values[0], values[1]).ToString();
    }

    public void ShowAddFloat()
    {
        float[] values = new float[2];
        for (int i = 0; i < 2; i++)
        {
            if (!float.TryParse(float_input[i].text, out values[i]))
            {
                float_input[i].image.color = new Color(1f, 0.3f, 0.3f);
                add_float_result.text = String.Empty;
                return;
            }
            else
                float_input[i].image.color = Color.white;
        }
        add_float_result.text = AddTwoFloats(values[0], values[1]).ToString();
    }
}
