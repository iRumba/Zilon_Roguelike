﻿using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных о выживании для человеческих персонажей.
    /// </summary>
    public sealed class HumanSurvivalData : ISurvivalData
    {
        private readonly ISurvivalRandomSource _randomSource;

        public HumanSurvivalData([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource)
        {
            if (personScheme == null)
            {
                throw new ArgumentNullException(nameof(personScheme));
            }

            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));

            Stats = new[] {
                new SurvivalStat(personScheme.Hp, 0, personScheme.Hp){
                    Type = SurvivalStatType.Health
                },
                CreateStat(SurvivalStatType.Satiety),
                CreateStat(SurvivalStatType.Water)
            };
        }

        public SurvivalStat[] Stats { get; }
        public bool IsDead { get; private set; }

        public event EventHandler<SurvivalStatChangedEventArgs> StatCrossKeyValue;
        public event EventHandler Dead;

        public void RestoreStat(SurvivalStatType type, int value)
        {
            ValidateStatChangeValue(value);

            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                ChangeStatInner(stat, value);
            }
        }

        public void DecreaseStat(SurvivalStatType type, int value)
        {
            ValidateStatChangeValue(value);

            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                ChangeStatInner(stat, -value);
            }
        }

        public void SetStatForce(SurvivalStatType type, int value)
        {
            var stat = Stats.SingleOrDefault(x => x.Type == type);
            if (stat != null)
            {
                stat.Value = value;
            }
        }

        public void Update()
        {
            foreach (var stat in Stats)
            {
                var roll = _randomSource.RollSurvival(stat);
                var successRoll = GetSuccessRoll();

                if (roll >= successRoll)
                {
                    ChangeStatInner(stat, -stat.Rate);
                }
            }
        }
        
        private void ValidateStatChangeValue(int value)
        {
            if (value == 0)
            {
                return;
            }

            if (value < 0)
            {
                throw new ArgumentException("Величина не может быть меньше 0.", nameof(value));
            }
        }

        private void ChangeStatInner(SurvivalStat stat, int value)
        {
            var oldValue = stat.Value;

            stat.Value += value;

            if (stat.KeyPoints != null)
            {
                CheckStatKeyPoints(stat, oldValue);
            }

            ProcessIfHealth(stat);
        }

        private void CheckStatKeyPoints(SurvivalStat stat, int oldValue)
        {
            var diff = RangeHelper.CreateNormalized(oldValue, stat.Value);

            var orientedKeyPoints = stat.KeyPoints;
            if (!diff.IsAcs)
            {
                orientedKeyPoints = stat.KeyPoints.Reverse().ToArray();
            }

            var crossedKeyPoints = new List<SurvivalStatKeyPoint>();
            foreach (var keyPoint in orientedKeyPoints)
            {
                if (diff.Contains(keyPoint.Value))
                {
                    crossedKeyPoints.Add(keyPoint);
                }
            }

            if (crossedKeyPoints.Any())
            {
                DoStatCrossKeyPoint(stat, crossedKeyPoints);
            }
        }

        /// <summary>
        /// Индивидуально обрабатывает характеристику, если это здоровье.
        /// </summary>
        /// <param name="stat"> Обрабатываемая характеристика. </param>
        private void ProcessIfHealth(SurvivalStat stat)
        {
            if (stat.Type != SurvivalStatType.Health)
            {
                return;
            }
            
            var hp = stat.Value;
            if (hp <= 0)
            {
                IsDead = true;
                DoDead();
            }
        }

        private void DoDead()
        {
            Dead?.Invoke(this, new EventArgs());
        }

        private static SurvivalStat CreateStat(SurvivalStatType type)
        {
            var stat = new SurvivalStat(150, -150, 300)
            {
                Type = type,
                Rate = 1,
                KeyPoints = new[]{
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Max, -100),
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Strong, -50),
                        new SurvivalStatKeyPoint(SurvivalStatHazardLevel.Lesser, 0)
                    }
            };
            return stat;
        }

        private void DoStatCrossKeyPoint(SurvivalStat stat, IEnumerable<SurvivalStatKeyPoint> keyPoints)
        {
            var args = new SurvivalStatChangedEventArgs(stat, keyPoints);
            StatCrossKeyValue?.Invoke(this, args);
        }


        private static int GetSuccessRoll()
        {
            // В будущем этот порог будет расчитываться, исходя из характеристик, перков и экипировки персонажа.
            return 4;
        }
    }
}