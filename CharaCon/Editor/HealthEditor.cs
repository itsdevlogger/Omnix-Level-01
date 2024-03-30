using MenuManagement.Editor;
using Omnix.CharaCon.HealthSystem;
using UnityEditor;

namespace Omnix.CharaCon.CustomEditors
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Health), true)]
    public class HealthEditor : BaseEditorWithGroups
    {
       
    }
}