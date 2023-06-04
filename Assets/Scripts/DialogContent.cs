using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogContent", order = 1)]
public class DialogContent : ScriptableObject
{
    public List<string> dialogLines;
}