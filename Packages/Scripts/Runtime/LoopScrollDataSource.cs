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

        public LoopScrollSendData(T[] listObjectsToFill)
        {
            this.listObjectsToFill = listObjectsToFill;
        }

        public override void ProvideData(PoolObject poolObject, int idx)
        {
            var comp = poolObject.GetComponent<IPoolObject<T>>();
            if (comp != null)
                comp.SetScrollData(idx, listObjectsToFill[idx]);
            else
                poolObject.transform.SendMessage("SetScrollData", listObjectsToFill[idx]);
        }
    }
}