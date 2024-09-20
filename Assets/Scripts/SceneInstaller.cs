using Resources;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private ResourcesDatabaseScriptableObject _resourcesDatabaseScriptableObject;
    [SerializeField] private ResourcePool _resourcePool;
    
    public override void InstallBindings()
    {
        Container
            .Bind<ResourcesDatabaseScriptableObject>()
            .FromScriptableObject(_resourcesDatabaseScriptableObject)
            .AsSingle();
        
        Container
            .Bind<ResourcePool>()
            .FromInstance(_resourcePool)
            .AsSingle();
    }
}
