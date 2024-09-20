using System;
using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(fileName = "ResourcesDatabase", menuName = "ScriptableObjects/ResourcesDatabase")]
    public class ResourcesDatabaseScriptableObject : ScriptableObject
    {
        [SerializeField] private ResourceModel[] _resourceModels;

        public ResourceModel GetResourceById(int id)
        {
            foreach (var resourceModel in _resourceModels)
            {
                if (resourceModel.Id == id) return resourceModel;
            }

            throw new Exception("There is no resource with id " + id + " in ResourcesDatabase");
        }
    }

    [Serializable]
    public class ResourceModel
    {
        public int Id;
        public GameObject ResourcePrefab;

        public ResourceModel(int id)
        {
            Id = id;
            ResourcePrefab = null;
        }
    }
}