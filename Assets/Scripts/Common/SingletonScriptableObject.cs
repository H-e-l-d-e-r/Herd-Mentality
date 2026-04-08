using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
{
    private static T s_instance; 
    public static T Instance
    {
        get
        {
            if(s_instance == null)
            {
                AsyncOperationHandle<T> operation = Addressables.LoadAssetAsync<T>(typeof(T).Name);
                s_instance = operation.WaitForCompletion();
            }

            return s_instance;
        }
    }
}