using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Resources
{
    public class ResourcePool : MonoBehaviour
    {
        private List<ResourceInstance> _activeResources = new();
        private List<ResourceInstance> _inactiveResources = new();
    
        [Inject] private ResourcesDatabaseScriptableObject _resourcesDatabase;

        public GameObject GetResourceVisual(int resourceId)
        {
            for (int i = 0; i < _inactiveResources.Count; i++)
            {
                if (_inactiveResources[i].ResourceModel.Id == resourceId)
                {
                    ResourceInstance resourceInstance = _inactiveResources[i];
                    _inactiveResources.RemoveAt(i);
                    _activeResources.Add(resourceInstance);
                
                    resourceInstance.VisualInstance.SetActive(true);
                    return resourceInstance.VisualInstance;
                }
            }

            ResourceModel resourceModel = _resourcesDatabase.GetResourceById(resourceId);
            GameObject newResource = Instantiate(resourceModel.ResourcePrefab);
            ResourceInstance newResourceInstance = new ResourceInstance(resourceModel, newResource);
            _activeResources.Add(newResourceInstance);
        
            return newResource;
        }

        public void ReturnResourceVisual(GameObject visualInstance)
        {
            ResourceInstance resourceToReturn = null;
            for (int i = 0; i < _activeResources.Count; i++)
            {
                if (_activeResources[i].VisualInstance == visualInstance)
                {
                    resourceToReturn = _activeResources[i];
                    _activeResources.RemoveAt(i);
                    break;
                }
            }

            if (resourceToReturn != null)
            {
                _inactiveResources.Add(resourceToReturn);
                resourceToReturn.VisualInstance.transform.parent = null;
                resourceToReturn.VisualInstance.SetActive(false);
            }
        }
    }
}