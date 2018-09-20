﻿using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема характиристик предмета, который можно экипировать на персонажа.
    /// </summary>
    public sealed class PropEquipSubScheme : SubSchemeBase, IPropEquipSubScheme
    {
        /// <summary>
        /// Мощь. Влияет на все характиристики предмета.
        /// </summary>
        public float Power { get; private set; }

        /// <summary>
        /// Ранг пробития брони.
        /// </summary>
        public int ApRank { get; private set; }

        /// <summary>
        /// Ранг брони.
        /// </summary>
        public int ArmorRank { get; private set; }

        /// <summary>
        /// Доля поглощения урона при равном ранге пробития и брони.
        /// </summary>
        /// <remarks>
        /// Зависит от Мощи.
        /// </remarks>
        public float Absorbtion { get; private set; }

        /// <summary>
        /// Идентификаторы действий, которые позволяет совершать предмет.
        /// </summary>
        public string[] ActSids { get; private set; }

        /// <summary>
        /// Типы слотов, в которые возможна экипировка предмета.
        /// </summary>
        public EquipmentSlotTypes[] SlotTypes { get; private set; }
    }
}
