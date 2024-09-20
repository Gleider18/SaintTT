using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Resources
{
    public class ResourceStorageController : MonoBehaviour
    {
        [SerializeField] private bool _isOutputStack;
        [SerializeField] private int _stackCapacity = 10;
        [SerializeField] private int _gridSize = 3;
        [SerializeField] private float _spacing = 1.0f;

        [Inject] private ResourcesDatabaseScriptableObject _resourcesDatabase;
        [Inject] private ResourcePool _resourcePool;
    
        private List<ResourceInstance> _resourceInstances = new();
    
        public void VisualizeStorage()
        {
            for (int i = 0; i < _resourceInstances.Count; i++)
            {
                ResourceInstance resourceInstance = _resourceInstances[i];

                int layer = i / (_gridSize * _gridSize);
                int positionInLayer = i % (_gridSize * _gridSize);
                int x = positionInLayer % _gridSize;
                int z = positionInLayer / _gridSize;

                float centeredX = (x - (_gridSize - 1) / 2f) * _spacing;
                float centeredZ = (z - (_gridSize - 1) / 2f) * _spacing;

                Vector3 position = new Vector3(centeredX, layer * 1.4f + 1f, centeredZ);
            
                resourceInstance.VisualInstance.transform.DOLocalMove(position, 0.3f);
                resourceInstance.VisualInstance.transform.DOLocalRotate(Vector3.zero, 0.3f);
            }
        }
    
        public void AddResource(int resourceId)
        {
            if (_resourceInstances.Count < _stackCapacity)
            {
                ResourceModel resourceModel = _resourcesDatabase.GetResourceById(resourceId);
                GameObject visual = _resourcePool.GetResourceVisual(resourceId);
                visual.transform.position = transform.position;
                ResourceInstance newResourceInstance = new ResourceInstance(resourceModel, visual);
            
                _resourceInstances.Add(newResourceInstance);
                newResourceInstance.VisualInstance.transform.SetParent(transform);
            }
        }

        public void RemoveResource(int id)
        {
            foreach (var resourceInstance in _resourceInstances)
            {
                if (resourceInstance.ResourceModel.Id == id)
                {
                    _resourceInstances.Remove(resourceInstance);
                    _resourcePool.ReturnResourceVisual(resourceInstance.VisualInstance);
                    return;
                }
            }
        }
    
        public void AddResourceInstance(ResourceInstance resourceInstance)
        {
            if (_resourceInstances.Count < _stackCapacity)
            {
                _resourceInstances.Add(resourceInstance);
                resourceInstance.VisualInstance.transform.SetParent(transform);
            }
        }

        public ResourceInstance TakeResourceInstance()
        {
            if (_resourceInstances.Count > 0)
            {
                ResourceInstance resourceInstance = _resourceInstances[_resourceInstances.Count - 1];
                _resourceInstances.RemoveAt(_resourceInstances.Count - 1);
                return resourceInstance;
            }

            return null;
        }

        public bool HasResource(int id)
        {
            foreach (var resourceInstance in _resourceInstances)
            {
                if (resourceInstance.ResourceModel.Id == id) return true;
            }
            return false;
        }

        public bool IsFull() =>  _resourceInstances.Count >= _stackCapacity;

        public bool IsOutputStack() => _isOutputStack;
    }

    public class ResourceInstance
    {
        public ResourceModel ResourceModel;
        public GameObject VisualInstance;

        public ResourceInstance(ResourceModel model, GameObject visualObject)
        {
            ResourceModel = model;
            VisualInstance = visualObject;
        }
    }
}