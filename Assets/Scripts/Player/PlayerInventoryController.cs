using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Resources;
using UnityEngine;

namespace Player
{
    public class PlayerInventoryController : MonoBehaviour
    {
        [SerializeField] private int _maxInventoryCapacity = 45;
        [SerializeField] private Transform _inventoryStackPosition;
        [SerializeField] private float _transferInterval = 1f;
        [SerializeField] private float _spacing = 0.3f;
        [SerializeField] private int _gridSize = 3;

        private List<ResourceInstance> _inventory = new();
        private ResourceStorageController _currentStorageController;
    
        private Coroutine _resourceTransferCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ResourceStack") && other.TryGetComponent(out ResourceStorageController storage))
            {
                _currentStorageController = storage;

                if (storage.IsOutputStack() && _inventory.Count < _maxInventoryCapacity) 
                    _resourceTransferCoroutine = StartCoroutine(CollectResourcesFromStack());
                else if (!storage.IsOutputStack() && _inventory.Count > 0) 
                    _resourceTransferCoroutine = StartCoroutine(TransferResourcesToStack());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("ResourceStack"))
            {
                if (_resourceTransferCoroutine != null)
                {
                    StopCoroutine(_resourceTransferCoroutine);
                    _resourceTransferCoroutine = null;
                }
                _currentStorageController = null;
            }
        }

        private IEnumerator CollectResourcesFromStack()
        {
            while (true)
            {
                if (_inventory.Count < _maxInventoryCapacity && _currentStorageController != null)
                {
                    var resourceInstance = _currentStorageController.TakeResourceInstance();
                    if (resourceInstance != null)
                    {
                        _inventory.Add(resourceInstance);
                        resourceInstance.VisualInstance.transform.SetParent(_inventoryStackPosition);
                        VisualizeInventory();
                        _currentStorageController.VisualizeStorage();
                    }
                }

                yield return new WaitForSeconds(_transferInterval);
            }
        }

        private IEnumerator TransferResourcesToStack()
        {
            while (true)
            {
                if (_inventory.Count > 0 && _currentStorageController != null)
                {
                    var resourceInstance = _inventory[^1];
                    _inventory.RemoveAt(_inventory.Count - 1);
                    VisualizeInventory();

                    _currentStorageController.AddResourceInstance(resourceInstance);
                    _currentStorageController.VisualizeStorage();
                }

                yield return new WaitForSeconds(_transferInterval);
            }
        }
    
        private void VisualizeInventory()
        {
            for (int i = 0; i < _inventory.Count; i++)
            {
                var resourceInstance = _inventory[i];

                // Рассчитываем позиции в центрированной паттерне 3x3
                int layer = i / (_gridSize * _gridSize); // Какой "слой" по оси Y
                int positionInLayer = i % (_gridSize * _gridSize); // Позиция в текущем слое 3x3
                int x = positionInLayer % _gridSize; // Позиция по X
                int z = positionInLayer / _gridSize; // Позиция по Z

                // Центрируем по x и z (чтобы значения были от -1 до 1 при gridSize = 3)
                float centeredX = (x - (_gridSize - 1) / 2f) * _spacing;
                float centeredZ = (z - (_gridSize - 1) / 2f) * _spacing;

                Vector3 position = new Vector3(centeredX, layer * _spacing + 0.12f, centeredZ);
            
                resourceInstance.VisualInstance.transform.DOLocalMove(position, 0.3f);
                resourceInstance.VisualInstance.transform.DOLocalRotate(Vector3.zero, 0.3f);
            }
        }
    }
}
