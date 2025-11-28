using UnityEngine;
public class SpawnObjectByPropertiesList : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject prefab;
    [SerializeField] private ScriptableObject[] properties;
    [SerializeField, HideInInspector] private GameObject[] instantiatedGameObjects;

    public GameObject[] InstantiatedGameObjects => instantiatedGameObjects;
    public Transform Parent => parent;
    public ScriptableObject[] GetPropreties() => properties;

    [ContextMenu(nameof(SpawnInEditMode))]
    public void SpawnInEditMode()
    {
        if (Application.isPlaying == true) return;

        GameObject[] allObjects = new GameObject[parent.childCount];

        for (int i = 0; i < parent.childCount; i++)
        {
            allObjects[i] = parent.GetChild(i).gameObject;
        }
        for (int i = 0; i < allObjects.Length; i++)
        {
            DestroyImmediate(allObjects[i]);
        }
        instantiatedGameObjects= new GameObject[properties.Length];
        for (int i = 0; i < properties.Length; i++)
        {
            GameObject gameObject = Instantiate(prefab, parent);
            IScriptableObjectProperty[] scriptableObjects = gameObject.GetComponents<IScriptableObjectProperty>();
            foreach (var so in scriptableObjects) { so.ApplyProperty(properties[i]); }
            instantiatedGameObjects[i] = gameObject;
        }
        ISpawnObjectsThrow iSpawnObjectsThrow;
        TryGetComponent(out iSpawnObjectsThrow);
        if(iSpawnObjectsThrow != null) iSpawnObjectsThrow.TakeGameObjects(instantiatedGameObjects);
    }
    public void SpawnBy(ScriptableObject[] properties)
    {
        GameObject[] allObjects = new GameObject[parent.childCount];
        this.properties = properties;

        for (int i = 0; i < parent.childCount; i++)
        {
            allObjects[i] = parent.GetChild(i).gameObject;
        }
        for (int i = 0; i < allObjects.Length; i++)
        {
            Destroy(allObjects[i]);
        }
        instantiatedGameObjects = new GameObject[this.properties.Length];
        for (int i = 0; i < this.properties.Length; i++)
        {
            GameObject gameObject = Instantiate(prefab, parent);
            IScriptableObjectProperty[] scriptableObjects = gameObject.GetComponents<IScriptableObjectProperty>();
            foreach (var so in scriptableObjects) { so.ApplyProperty(this.properties[i]); }
            instantiatedGameObjects[i] = gameObject;
        }
        ISpawnObjectsThrow iSpawnObjectsThrow;
        TryGetComponent(out iSpawnObjectsThrow);
        if (iSpawnObjectsThrow != null) iSpawnObjectsThrow.TakeGameObjects(instantiatedGameObjects);
    }
}