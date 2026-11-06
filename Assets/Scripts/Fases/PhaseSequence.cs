using System.Collections.Generic;
using UnityEngine;

// Lista oficial de fases, na ordem certa — substitui a fragilidade
// de depender da ordem/índice nas Build Settings.
[CreateAssetMenu(fileName = "PhaseSequence", menuName = "Alongside You/Phase Sequence")]
public class PhaseSequence : ScriptableObject
{
    public List<string> phaseSceneNames = new();
}