using TMPro;
using UnityEngine;

public class ResetText : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameText;
    [SerializeField] private TMP_InputField idText;
    [SerializeField] private TMP_InputField passText;

    public void OnClick()
    {
        if (nameText != null)
        {
            nameText.text = "";
        }
        if (idText != null)
        {
            idText.text = "";
        }
        if (passText != null)
        {
            passText.text = "";
        }
    }
}
