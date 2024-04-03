using System.Collections.Generic;
using UnityEngine;

namespace Omnix.CharaCon.InteractionSystem
{
    /// <summary> Highlight an interactable by changing it's material  </summary>
    public class InMatChange : IndicatorBase
    {
        [SerializeField] private Material _material;
        private readonly Dictionary<Renderer, Material> _oldMaterials = new Dictionary<Renderer, Material>();

        public override void Highlight(InteractableBase interactable, AgentInteraction player)
        {
            if (_oldMaterials.Count > 0)
            {
                Unhighlight(interactable, player);
            }

            foreach (Renderer child in GetComponentsInChildren<Renderer>())
            {
                _oldMaterials.Add(child, child.material);
                child.material = _material;
            }
        }

        public override void Unhighlight(InteractableBase interactable, AgentInteraction player)
        {
            foreach (Renderer child in GetComponentsInChildren<Renderer>())
            {
                if (_oldMaterials.TryGetValue(child, out Material mat))
                {
                    child.material = mat;
                }
            }
            
            _oldMaterials.Clear();
        }
    }

}