using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallback1 : MonoBehaviour, ILoopScrollElement<Data>
{
    public Image image;
    public Text text;

    void SetScrollData(int index)
    {
        this.image.color = Color.white;
        this.text.text = index.ToString();
    }
    
    public void SetScrollData(int index, Data data, params object[] other)
    {
        this.image.color = data.color;
        this.text.text = data.text;
    }
}