using System.Collections.Generic;
using UnityEngine;

namespace Roguelite
{
    [CreateAssetMenu(fileName = "New Runtime Buff Set", menuName = "Roguelite/Runtime Buff Set")]
    public class RogueliteRuntimeBuffSet : ScriptableObject
    {
        public List<RogueliteRuntimeSelectData> tempBuffDatas;
    }
}