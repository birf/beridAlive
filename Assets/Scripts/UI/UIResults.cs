using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIResults : MonoBehaviour
{
    //public enum ResultOptions { WIN, LOSE }

    [System.Serializable]
    public class TextInfo
    {
        public ResultOptions resultOptions;
        public string textToDisplay;
        public Color textColor;
    }

    public enum ResultOptions { WIN, LOSE }

    [SerializeField] List<TextInfo> textInfos;
    [SerializeField] TextMeshProUGUI resultTextMeshPro;


    [SerializeField] ResultOptions currentResult;

    // Start is called before the first frame update
    void Start()
    {
        RetrieveResultOption(textInfos, currentResult);
    }

    void RetrieveResultOption(List<TextInfo> textInfos, ResultOptions result)
    {
        TextInfo selectedTextInfo = textInfos[0];
        for (int i = 0; i < textInfos.Count; i++)
        {
            if (textInfos[i].resultOptions == result)
            {
                selectedTextInfo = textInfos[i];
            }
        }
        DisplayText(resultTextMeshPro, selectedTextInfo);
    }

    void DisplayText(TextMeshProUGUI textMeshProUGUI, TextInfo textInfo)
    {
        textMeshProUGUI.text = textInfo.textToDisplay;
        textMeshProUGUI.color = textInfo.textColor;
    }
}
