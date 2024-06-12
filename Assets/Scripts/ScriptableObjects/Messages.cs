using UnityEngine;

[CreateAssetMenu(fileName = "Message", menuName = "ScriptableObjects/MessageSO")]
public class Messages : ScriptableObject
{
    [TextArea(3, 4)]public string message;
    public AudioClip voice;
}
