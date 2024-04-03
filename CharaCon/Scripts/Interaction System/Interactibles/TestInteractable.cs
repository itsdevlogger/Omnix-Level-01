namespace Omnix.CharaCon.InteractionSystem
{
    public class TestInteractable : InteractableBase
    {
        public override void OnInteract(AgentInteraction agentInteraction)
        {
            agentInteraction.EndInteractionDelayed(1f);
        }

        public override void OnUnInteract(AgentInteraction agentInteraction)
        {
            
        }
    }
}