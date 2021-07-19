using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallback3 : MonoBehaviour
{
    public Text text;

    void SetScrollData(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
    }
}