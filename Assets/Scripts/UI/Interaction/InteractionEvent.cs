using System;
using System.Linq.Expressions;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{

    [SerializeField] private DialogueEvent dialogue;
    private DialogueManager DM;

    public Dialogue[] GetDialogues()
    {
        dialogue.dialogues = DatabaseManager.instance.GetDialogue((int)dialogue.line.x, (int)dialogue.line.y);
        return dialogue.dialogues;
    }

}
