using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamColor : MonoBehaviour
{
    [SerializeField]
    private List<Color> teamColors = new List<Color>();
    
    private List<Color> usedTeamColors = new List<Color>();


    public Color GetColor()
    {
        List<Color> availableColors = teamColors.Except(usedTeamColors).ToList();

        if (availableColors.Count() == 0)
        {
            availableColors = teamColors;
        }

        int selectedIndex = Random.Range(0, availableColors.Count());

        Color selectedColor = availableColors[selectedIndex];

        usedTeamColors.Add(selectedColor);

        return selectedColor;
    }
}
