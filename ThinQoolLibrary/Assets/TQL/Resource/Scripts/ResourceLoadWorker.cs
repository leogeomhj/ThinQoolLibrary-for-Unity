using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TQL.Resource
{
    public class ResourceLoadWorker : MonoBehaviour
    {
        internal struct LoadData
        {
            public Action<UnityEngine.Object> callback;
            public Type type;
            public string path;
            public string assetName;
            public bool asyncLoading;
            public bool fromAssetBundle;

            public LoadData(Action<UnityEngine.Object> callback_, Type type_, string path_, string assetName_, bool asyncLoading_, bool fromAssetBundle_)
            {
                callback = callback_;
                type = type_;
                path = path_;
                assetName = assetName_;
                asyncLoading = asyncLoading_;
                fromAssetBundle = fromAssetBundle_;
            }
        }

        private Coroutine coroutine = null;
        private List<LoadData> workList = new List<LoadData>();
        private int workingCount = 0;

        public bool IsMainWorker { get; set; }

        public void StartWork()
        {
            if (coroutine == null)
            {
                if (IsMainWorker)
                {
                    coroutine = StartCoroutine(DoMainWork());
                }
                else
                {
                    coroutine = StartCoroutine(DoSubWork());
                }
            }
        }

        IEnumerator DoMainWork()
        {
            while (true)
            {
                yield return null;
            }
        }

        IEnumerator DoSubWork()
        {
            while (true)
            {
                yield return null;
            }
        }

        private void OnDestroy()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
