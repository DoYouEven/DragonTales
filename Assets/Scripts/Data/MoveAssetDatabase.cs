using UnityEngine;
using System.Collections;

public class MoveAssetDatabase : ScriptableObject
{

    public MoveAssetData[] skills;

    /// <summary>
    /// Get the specified SpellInfo by index.
    /// </summary>
    /// <param name="index">Index.</param>
    public MoveAssetData Get(int index)
    {
        return (skills[index]);
    }

    /// <summary>
    /// Gets the specified SpellInfo by ID.
    /// </summary>
    /// <returns>The SpellInfo or NULL if not found.</returns>
    /// <param name="ID">The spell ID.</param>
    public MoveAssetData GetByID(int ID)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].ID == ID)
                return skills[i];
        }

        return null;
    }
}