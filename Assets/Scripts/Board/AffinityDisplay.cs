using System;
using TMPro;
using UnityEngine;

namespace Board
{
    public class AffinityDisplay : MonoBehaviour
    {
        private Player player;
        private TextMeshProUGUI textMesh;

        public void Awake()
        {
            player = GetComponentInParent<Player>();
            textMesh = GetComponent<TextMeshProUGUI>();
        }
        public void Update()
        {
            var displayText = "Af:\n";
            foreach (var entry in player.affinities)
            {
                displayText += $"{entry.Key}: {entry.Value}\n";
            }
            textMesh.text = displayText;
            // test 2 3
        }
    }
}