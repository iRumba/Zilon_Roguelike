﻿using System;
using Zilon.Core.Components;
using Zilon.Core.Tactics;

namespace Zilon.Core.Persons
{
    public class SurvivalStatHazardEffect : IPersonEffect, IActorStateEffect
    {
        private SurvivalStatHazardLevel _level;

        public SurvivalStatHazardEffect(SurvivalStatType type, SurvivalStatHazardLevel level)
        {
            Type = type;
            Level = level;

            Rules = CalcRules();
        }

        public SurvivalStatType Type { get; }

        public SurvivalStatHazardLevel Level
        {
            get => _level;
            set
            {
                _level = value;

                Rules = CalcRules();

                Changed?.Invoke(this, EventArgs.Empty);
            }
        }

        public EffectRule[] Rules { get; private set; }

        public event EventHandler Changed;

        public void Apply(IActorState actorState)
        {
            if (Level == SurvivalStatHazardLevel.Max)
            {
                actorState.TakeDamage(5);
            }
        }

        public void Update()
        {
            // На персонажа нет влияния
        }

        private EffectRule[] CalcRules()
        {
            PersonRuleLevel ruleLevel;
            switch (Level)
            {
                case SurvivalStatHazardLevel.Lesser:
                    ruleLevel = PersonRuleLevel.Lesser;
                    break;

                case SurvivalStatHazardLevel.Strong:
                    ruleLevel = PersonRuleLevel.Normal;
                    break;

                case SurvivalStatHazardLevel.Max:
                    ruleLevel = PersonRuleLevel.Grand;
                    break;

                default:
                    throw new NotSupportedException("Неизветный уровень угрозы выживания.");
            }

            return new[] {
                new EffectRule{
                    Level = ruleLevel,
                    StatType = SkillStatType.Ballistic
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = SkillStatType.Medic
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = SkillStatType.Melee
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = SkillStatType.Psy
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = SkillStatType.Social
                },
                new EffectRule{
                    Level = ruleLevel,
                    StatType = SkillStatType.Tech
                },
            };
        }

        public override string ToString()
        {
            return $"{Level} {Type}";
        }
    }
}
