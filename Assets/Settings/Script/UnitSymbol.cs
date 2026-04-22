using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitSymbol", menuName = "Slot Machine/Unit Symbol")]
public class UnitSymbol : ScriptableObject
{
    public string unitName;
    public Sprite unitIcon;
    public int spawnWeight;
    public int baseDamage;
}
