using System;
using Gfen.Game.Logic;
using UnityEngine;

namespace Gfen.Game.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "EntityCategoryConfig", menuName = "babaisyou/EntityCategoryConfig", order = 0)]
    public class EntityCategoryConfig : ScriptableObject
    {
        public EntityCategory entityCategory;

        // default attributes, default properties
        public AttributeCategory[] inherentAttributeCategories;
    }
}
