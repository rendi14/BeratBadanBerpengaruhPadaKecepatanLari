using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PusingYaAllah;
using System.IO;
using UnityEditor;

public class InputForm : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField _inputX;
    public TextAsset jsonText;
    public void InputSimulationData()
    {
        Constanta.nilaiX = int.Parse(_inputX.text);
    }

    public void OpenTXTFile()
    {
        Constanta.jsonFile = jsonText;
        Debug.Log(Constanta.jsonFile);
        //Application.OpenURL(jsonText);
    }
}
