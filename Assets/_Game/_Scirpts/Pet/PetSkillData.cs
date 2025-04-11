using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetSkillData", menuName = "PetSystem/PetSkillData")]
public class PetSkillData : ScriptableObject
{
    public string skillName;
    [TextArea(2, 5)]
    public string description;
    public float power;
    public float cooldown;
}
