using UnityEngine;
using System.ComponentModel;
using UnityEditor;

public enum CharacterName { NPC, Player }

[System.Serializable]
public class Dialogue
{
    public CharacterName name;
    public ImageSentence[] phrases;
}

[System.Serializable]
public class ImageSentence
{
    
    public string sentences;
    public Sprite sprite;
}