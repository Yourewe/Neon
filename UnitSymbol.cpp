using UnityEngine;

// This line lets us right-click in Unity to create new Symbols easily!
[CreateAssetMenu(fileName = "NewUnitSymbol", menuName = "Slot Machine/Unit Symbol")]
public class UnitSymbol : ScriptableObject
{
    public string unitName;          // e.g., "Street Samurai"
    public Sprite unitIcon;          // The neon graphic
    public int spawnWeight;          // Higher number = more common. Lower = rare!
    public int baseDamage;

    // In Python, this would be a dataclass or a dictionary. 
    // Here, it's a reusable data asset that lives in your project folders.
}