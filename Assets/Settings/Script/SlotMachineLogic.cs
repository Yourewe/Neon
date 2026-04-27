using System.Collections; // Crucial for Coroutines!
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public float spacing = 1.5f;

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;
    private int currentScore = 0;

    // --- NEW VARIABLES ---
    public float spinDelay = 0.5f;     // How long to wait between each column
    private bool isSpinning = false;   // The Safety Lock!
    // ---------------------

    private List<GameObject> spawnedVisuals = new List<GameObject>();

    void Start()
    {
        slotGrid = new UnitSymbol[columns, rows];
    }

    // This is the method your UI Button calls
    public void SpinLogic()
    {
        // Safety check: If the reels are already spinning, ignore the button click!
        if (isSpinning)
        {
            return;
        }

        // Start the special timed function
        StartCoroutine(SpinRoutine());
    }

    // The Coroutine (IEnumerator) allows us to pause time. 
    private IEnumerator SpinRoutine()
    {
        isSpinning = true; // Lock the button
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
                Vector2 spawnPos = new Vector2(
                    gridStartPosition.position.x + (x * spacing),
                    gridStartPosition.position.y - (y * spacing) // Minus Y so it builds downwards
                );

                GameObject newVisualSlot = Instantiate(visualSlotPrefab, spawnPos, Quaternion.identity);
                newVisualSlot.GetComponent<SpriteRenderer>().sprite = slotGrid[x, y].unitIcon;
                spawnedVisuals.Add(newVisualSlot);
            }

            // --- THE MAGIC LINE ---
            // Pause the code here for half a second before looping to the next column
            yield return new WaitForSeconds(spinDelay);
        }

        // Wait to check for wins until ALL reels have stopped dropping
        CheckHorizontalWins();
        CheckDiagonalWins();

        isSpinning = false; // Unlock the button so the player can spin again!
    }

    // --- The Weighted Probability Math ---
    private UnitSymbol GetRandomSymbol()
    {
        int totalWeight = 0;

        foreach (UnitSymbol symbol in availableSymbols)
        {
            totalWeight += symbol.spawnWeight;
        }

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (UnitSymbol symbol in availableSymbols)
        {
            currentWeight += symbol.spawnWeight;
            if (randomValue < currentWeight)
            {
                return symbol;
            }
        }

        return availableSymbols[0];
    }

    // --- The Win Logic ---
    public void CheckHorizontalWins()
    {
        Debug.Log("--- CHECKING FOR WINS ---");

        for (int y = 0; y < rows; y++)
        {
            UnitSymbol firstSymbol = slotGrid[0, y];
            bool isWinningRow = true;

            for (int x = 1; x < columns; x++)
            {
                if (slotGrid[x, y] != firstSymbol)
                {
                    isWinningRow = false;
                    break;
                }
            }

            if (isWinningRow)
            {
                int totalDamage = firstSymbol.baseDamage * columns;
                Debug.Log($"⭐ WINNER! Row {y} matched 3 {firstSymbol.unitName}s! Total Damage Dealt: {totalDamage} ⭐");

                // Hooked up the score here!
                AddToScore(totalDamage);
            }
        }
    }

    // --- Advanced Win Logic ---
    public void CheckDiagonalWins()
    {
        // 1. Check Top-Left to Bottom-Right ( \ )
        UnitSymbol centerSymbol = slotGrid[1, 1]; // The middle slot is key!

        if (slotGrid[0, 0] == centerSymbol && slotGrid[2, 2] == centerSymbol)
        {
            int totalDamage = centerSymbol.baseDamage * 3;
            Debug.Log($"💥 CRITICAL HIT! Diagonal Match (\\) of {centerSymbol.unitName}s! Damage: {totalDamage} 💥");

            // Hooked up the score here!
            AddToScore(totalDamage);
        }

        // 2. Check Bottom-Left to Top-Right ( / )
        if (slotGrid[0, 2] == centerSymbol && slotGrid[2, 0] == centerSymbol)
        {
            int totalDamage = centerSymbol.baseDamage * 3;
            Debug.Log($"💥 CRITICAL HIT! Diagonal Match (/) of {centerSymbol.unitName}s! Damage: {totalDamage} 💥");

            // Hooked up the score here!
            AddToScore(totalDamage);
        }
    }

    // --- The Score Hookup ---
    private void AddToScore(int damageToAdd)
    {
        currentScore += damageToAdd;
        if (scoreText != null)
        {
            scoreText.text = "Total Damage: " + currentScore.ToString();
        }
    }
}