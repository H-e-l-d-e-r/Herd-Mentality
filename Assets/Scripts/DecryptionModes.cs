using System;
using Unity.Multiplayer.Center.Common;
using UnityEngine;

[Serializable]
public class DecryptionsResults
{
    public delegate string GetResultDelegate(string content, int modifier);

    public string Content => InternalGetContent(p_content, 0);

    public bool DoesAskModifier
    {
        get => Mode == DecryptionModes.FromCaesar;
    }

    public DecryptionModes Mode;
    public GetResultDelegate InternalGetContent;

    [TextArea]
    [SerializeField]
    protected string p_content;

    public DecryptionsResults()
    {
        InternalGetContent = DefaultDecryption;
    }

    public DecryptionsResults(DecryptionModes mode, string value) : this()
    {
        Mode = mode;
        p_content = value;
    }

    public string GetContent() => InternalGetContent(p_content, 0); 
    public string GetContent(int modifier) => InternalGetContent(p_content, modifier); 

    private string DefaultDecryption(string content, int offset)
    {
        if (DoesAskModifier)
        {
            return Mode switch
            {
                DecryptionModes.FromCaesar => DoCaesarDecryption(content, offset),
                _ => content
            };
        }

        return content;
    }

    private string DoCaesarDecryption(string content, int offset)
    {
        string newValue = new string("");
        foreach (char item in content)
        {
            if (char.IsLetter(item))
            {
                // do caesar convertion
                int ascii = Convert.ToInt32(char.ToLower(item));
                int cipher = (ascii + offset - 0x61) % 26;

                // redux upper or lower characters by using there ascii table
                // offsets
                cipher = char.IsUpper(item) ? (0x41 + cipher) : (0x61 + cipher);

                newValue += Convert.ToChar(cipher);
            }
            else
            {
                // non letter items aren't converted
                newValue += item;
            }
        }

        return newValue;
    }
}

public enum DecryptionModes 
{
    FromAudio,
    FromMorse,
    FromInvertAudio,
    FromCaesar
}