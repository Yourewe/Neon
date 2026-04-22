using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI.Table;

public class SlotMachineLogic : MonoBehaviour
{
    [Header("Machine Settings")]
    public int rows = 3;
    public int columns = 3;
    public List<UnitSymbol> availableSymbols;
    private UnitSymbol[,] slotGrid;

    [Header("Visual Settings")]
    public GameObject visualSlotPrefab;
    public Transform gridStartPosition;
    public float spacing = 1.5F;

    private List<GameObject> spawnedVisuals = new List<GameObject>();

    void Start()
    {
        // Initialize the empty 2D array (grid)
        slotGrid = new UnitSymbol[columns, rows];

        // Spin the machine immediately when the game starts just to test
        SpinLogic();
    }

    public void SpinLogic()
    {
        Debug.Log("--- NEW SPIN ---");

        // 1. Clear out the old visual symbols from the previous spin
        foreach (GameObject oldSlot in spawnedVisuals)
        {
            Destroy(oldSlot);
        }
        spawnedVisuals.Clear();

        // 2. Loop through and build the new grid
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Give the data grid a symbol
                slotGrid[x, y] = GetRandomSymbol();

                // --- THE VISUAL HOOKUP ---
                // Calculate where in the world this specific slot should go
                Vector2 spawnPos = new Vector2(
                    gridStartPosition.position.x + (x * spacing),
                    gridStartPosition.position.y - (y * spacing) // Minus Y so it builds downwards
                );

                // Spawn the blank prefab at that exact position
                GameObject newVisualSlot = Instantiate(visualSlotPrefab, spawnPos, Quaternion.identity);

                // Paint the correct icon onto the blank prefab
                newVisualSlot.GetComponent<SpriteRenderer>().sprite = slotGrid[x, y].unitIcon;

                // Save it to our list so we can destroy it next time
                spawnedVisuals.Add(newVisualSlot);
            }
        }

        CheckHorizontalWins();
    }

    // --- The Weighted Probability Math ---
    private UnitSymbol GetRandomSymbol()
    {
        int totalWeight = 0;

        // Add up the spawn weights of all symbols in the list
        foreach (UnitSymbol symbol in availableSymbols)
        {
            totalWeight += symbol.spawnWeight;
        }

        // Pick a random number based on that total
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        // Determine which symbol won the roll
        foreach (UnitSymbol symbol in availableSymbols)
        {
            currentWeight += symbol.spawnWeight;
            if (randomValue < currentWeight)
            {
                return symbol;
            }
        }

        // Fallback safety (should rarely hit this if weights are correct)
        return availableSymbols[0];

    }
    // --- The Win Logic ---
    public void CheckHorizontalWins()
    {
        Debug.Log("--- CHECKING FOR WINS ---");

        // We check row by row (the Y axis)
        for (int y = 0; y < rows; y++)
        {
            // Grab the first symbol in the current row
            UnitSymbol firstSymbol = slotGrid[0, y];
            bool isWinningRow = true;

            // Compare it against the rest of the columns in this row (starting at column 1)
            for (int x = 1; x < columns; x++)
            {
                if (slotGrid[x, y] != firstSymbol)
                {
                    isWinningRow = false;
                    break; // Optimization: Stop checking this row as soon as one doesn't match
                }
            }

            // If the boolean survived as true, the whole row matched!
            if (isWinningRow)
            {
                // Calculate the total damage/reward
                int totalDamage = firstSymbol.baseDamage * columns;
                Debug.Log($"⭐ WINNER! Row {y} matched 3 {firstSymbol.unitName}s! Total Damage Dealt: {totalDamage} ⭐");
            }
        }
    }
}