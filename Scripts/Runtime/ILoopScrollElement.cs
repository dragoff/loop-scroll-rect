namespace UnityEngine.UI
{
    public interface ILoopScrollElement<in T>
    {
        void SetScrollData(int index, T data, params object[] other);
    }
}