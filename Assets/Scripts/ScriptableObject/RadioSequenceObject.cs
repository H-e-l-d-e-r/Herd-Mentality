using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Sequence", menuName = "Herd Mentality/Sequence")]
public class RadioSequenceObject : CollectibleObject
{
    public VinylObject[] Blocs;

    public override string ToString()
    {
        string str = $"{Name} (";
        for (int i = 0; i < Blocs.Length; i++)
        {
            if(i > 0)
            {
                str += " + ";
            }

            str += Blocs[i].Name;
        }

        str += ")";
        
        return str;
    }
}