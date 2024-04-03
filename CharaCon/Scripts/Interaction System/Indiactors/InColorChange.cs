using System.Collections.Generic;
using UnityEngine;
namespace Omnix.CharaCon.InteractionSystem
{
    /// <summary> Highlight an interactable by changing it's material color  </summary>
    public class InColorChange : IndicatorBase
    {
        [SerializeField] private Color _color;
        private readonly Dictionary<Renderer, Color> _oldColors = new Dictionary<Renderer, Color>();
        
        public override void Highlight(InteractableBase interactable, AgentInteraction player)
        {
            if (_oldColors.Count > 0)
            {
                Unhighlight(interactable, player);
            }

            foreach (Renderer child in GetComponentsInChildren<Renderer>())
            {
                _oldColors.Add(child, child.material.color);
                child.material.color = _color;
            }
        }

        public override void Unhighlight(InteractableBase interactable, AgentInteraction player)
        {
            foreach (Renderer child in GetComponentsInChildren<Renderer>())
            {
                if (_oldColors.TryGetValue(child, out Color col))
                {
                    child.material.color = col;
                }
            }
            
            _oldColors.Clear();
        }
    }
}