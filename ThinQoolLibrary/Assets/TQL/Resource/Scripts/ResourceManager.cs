using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TQL.Singleton;

namespace TQL.Resource
{
    public class ResourceManager : MonoBehaviourSingleton<ResourceManager>
    {
        private ResourceLoadWorker mainWorker = null;
        private List<ResourceLoadWorker> subWorkers = new List<ResourceLoadWorker>();

        private void Start()
        {
            CreateMainWorker();
        }

        public void CreateMainWorker()
        {
            if (mainWorker == null)
            {
                GameObject go = new GameObject();
                go.name = "MainWorker";
                go.transform.parent = transform;

                mainWorker = go.AddComponent<ResourceLoadWorker>();
                mainWorker.IsMainWorker = true;

                mainWorker.StartWork();
            }
        }

        private ResourceLoadWorker CreateSubWorker()
        {
            GameObject go = new GameObject();
            go.name = "SubWorker" + subWorkers.Count.ToString();
            go.transform.parent = transform;

            ResourceLoadWorker subWorker = go.AddComponent<ResourceLoadWorker>();
            subWorker.IsMainWorker = false;
            subWorkers.Add(subWorker);

            return subWorker;
        }

        public void DestroySubWorker(ResourceLoadWorker subWorker)
        {
            if (subWorkers.Remove(subWorker) == true)
            {
                Destroy(subWorker.gameObject);
            }
        }
    }
}
