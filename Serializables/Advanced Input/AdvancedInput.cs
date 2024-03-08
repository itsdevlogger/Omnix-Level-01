using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

namespace Omnix.Inputs
{
    [Serializable]
    public class AdvancedInput
    {
        [SerializeField] private List<UnitInput> combinations;
        
        public bool IsPressed()
        {
            foreach (UnitInput comb in combinations)
            {
                if (comb.IsPressed()) return true;
            }

            return false;
        }

        public bool GetPressedCombo(out UnitInput combo)
        {
            foreach (UnitInput comb in combinations)
            {
                if (comb.IsPressed())
                {
                    combo = comb;
                    return true;
                }
            }

            combo = null;
            return false;
        }
        
        public IEnumerator<UnitInput> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (UnitInput comb in combinations)
            {
                builder.Append($"{comb} || ");
            }
            return builder.ToString();
        }
    }
}
