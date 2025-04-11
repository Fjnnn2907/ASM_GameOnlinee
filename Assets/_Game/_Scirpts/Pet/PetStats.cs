using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PetStatData", menuName = "PetSystem/PetStatData")]
public class PetStats: ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public int idPet;
    public string petName;
    public Sprite petIcon;

    [Header("Chỉ số cơ bản")]
    public float HP;
    public float Attack;
    public float Defense;
    public float MoveSpeed;
    public float AttackSpeed;
    public float CritChance;
    public float CritDamage;

    [Header("Chỉ số đặc biệt")]
    public List<SpecialStat> specialStats;

    [Header("Kỹ năng đặc biệt")]
    public List<PetSkillData> specialSkills;

}
