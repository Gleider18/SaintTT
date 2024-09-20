using System.Collections;
using Resources;
using TMPro;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] private ResourceStorageController _inputStorageController;
    [SerializeField] private ResourceStorageController _outputStorageController;
    [SerializeField] private int[] _requiredResourcesIdToProduce;
    [SerializeField] private float _productionTime;
    [SerializeField] private int _producedResourceId;
    [SerializeField] private TextMeshProUGUI _productionReportText;
    [SerializeField] private TextMeshProUGUI _buildingIdText;

    private void Start()
    {
        _buildingIdText.text = _producedResourceId.ToString();
        if (CanProduce())
        {
            _productionReportText.color = Color.green;
            _productionReportText.text = "B" + _producedResourceId + " working";
        }
        else _productionReportText.color = Color.red;
        
        StartCoroutine(ProduceResource());
    }

    private IEnumerator ProduceResource()
    {
        while (true)
        {
            yield return new WaitForSeconds(_productionTime);
            if (CanProduce())
            {
                _productionReportText.color = Color.green;
                _productionReportText.text = "B" + _producedResourceId + " working";
                foreach (var id in _requiredResourcesIdToProduce) _inputStorageController.RemoveResource(id);
                _inputStorageController.VisualizeStorage();

                _outputStorageController.AddResource(_producedResourceId);
                _outputStorageController.VisualizeStorage();
            }
            else _productionReportText.color = Color.red;
        }
    }

    private bool CanProduce()
    {
        if (_outputStorageController.IsFull())
        {
            _productionReportText.text = "B" + _producedResourceId + " stopped: storage full";
            return false;
        }

        foreach (var id in _requiredResourcesIdToProduce)
        {
            if (!_inputStorageController.HasResource(id))
            {
                _productionReportText.text = "B" + _producedResourceId + " stopped: not enough " + id + " resource";
                return false;
            }
        }

        return true;
    }
}
