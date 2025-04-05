using LDtkUnity;
using UnityEngine;

namespace DefaultNamespace
{
    public class DialogueTrigger : MonoBehaviour, ILDtkImportedFields
    {
        public string content;
        //public Dialogue dialogue;
        //public bool triggerOnce = true;
        //private bool hasTriggered = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                player.SetDialogueText(content);
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                player.SetDialogueText(null);
            }
        }

        public void OnLDtkImportFields(LDtkFields fields)
        {
            content = fields.GetMultiline("msg");
        }
    }
}