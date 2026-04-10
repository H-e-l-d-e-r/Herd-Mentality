using UnityEngine;

[CreateAssetMenu(fileName = "Collectible", menuName = "Herd Mentality/Collectible")]
public class CollectibleObject : ScriptableObject
{
    [Header("Meta")]
    public string Name;

    [TextArea]
    public string Description;
}
