// (c) https://github.com/qiankanglai/LoopScrollRect
// modified version by dragoff

using SG;

namespace UnityEngine.UI
{
    public abstract class LoopScrollDataSource
    {
        public abstract void ProvideData(PoolObject poolObject, int idx);
    }

    public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
        public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

        public override void ProvideData(PoolObject poolObject, int idx)
        {
            poolObject.transform.SendMessage("SetScrollData", idx);
        }
    }

    public class LoopScrollSendData<T> : LoopScrollDataSource
    {
        readonly T[] listObjectsToFill;
        private readonly object[] other;
        public LoopScrollSendData(T[] listObjectsToFill, params object[] other)
        {
            this.listObjectsToFill = listObjectsToFill;
            this.other = other;
        }

        public override void ProvideData(PoolObject poolObject, int idx)
        {
            var comp = poolObject.GetComponent<ILoopScrollElement<T>>();
            if (comp != null)
                comp.SetScrollData(idx, listObjectsToFill[idx], other);
            else
                poolObject.transform.SendMessage("SetScrollData", listObjectsToFill[idx]);
        }
    }
}