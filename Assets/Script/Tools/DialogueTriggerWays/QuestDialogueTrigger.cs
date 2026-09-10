using Script.Tools.DialogueTriggerWays.BaseClass;
using Script.Tools.Managers;

namespace Script.Tools.DialogueTriggerWays
{
    public class QuestDialogueTrigger : BaseDialogueTrigger
    {
        void Start()
        {
            EventsManager.OnDialogueManager += HandleQuestComplete;
        }
        void OnDestroy()
        {
            EventsManager.OnDialogueManager -= HandleQuestComplete;
        }
        
        void HandleQuestComplete()
        {
            TriggerDialogue();
        }
    }
}