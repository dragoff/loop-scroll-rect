// (c) https://github.com/qiankanglai/LoopScrollRect
// modified version by dragoff

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource
    {
        public GameObject Prefab;
        public string PrefabNameFromResources;
        public bool IsFromResources = false;
        public int InitPoolSize = 5;

        private bool inited = false;

        public virtual GameObject GetObject()
        {
            var poolObj = IsFromResources
                ? new SG.PoolData(PrefabNameFromResources, null)
                : new SG.PoolData(null, Prefab);

            if (!inited)
            {
                SG.ResourceManager.Instance.InitPool(poolObj, InitPoolSize);
                inited = true;
            }

            return SG.ResourceManager.Instance.GetObjectFromPool(poolObj);
        }

        public virtual void ReturnObject(Transform go)
        {
            go.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
        }
    }
}