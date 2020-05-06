﻿using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Schemes;

namespace Zilon.Core.PersonModules
{
    public sealed class HumanSurvivalModule : SurvivalModuleBase
    {
        private readonly IPersonScheme _personScheme;
        private readonly ISurvivalRandomSource _randomSource;

        public HumanSurvivalModule([NotNull] IPersonScheme personScheme,
            [NotNull] ISurvivalRandomSource randomSource) : base(GetStats(personScheme))
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));

            foreach (var stat in Stats)
            {
                stat.Changed += Stat_Changed;
            }
        }

        public HumanSurvivalModule([NotNull] IPersonScheme personScheme,
            [NotNull] IEnumerable<SurvivalStat> stats,
            [NotNull] ISurvivalRandomSource randomSource) : base(stats.ToArray())
        {
            _personScheme = personScheme ?? throw new ArgumentNullException(nameof(personScheme));
            _randomSource = randomSource ?? throw new ArgumentNullException(nameof(randomSource));
        }

        private static SurvivalStat[] GetStats([NotNull] IPersonScheme personScheme)
        {
            if (personScheme is null)
            {
                throw new ArgumentNullException(nameof(personScheme));
            }
            // Устанавливаем характеристики выживания персонажа
            var statList = new List<SurvivalStat>();
            SetHitPointsStat(personScheme, statList);

            // Выставляем сытость/упоённость
            if (personScheme.SurvivalStats != null)
            {
                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Satiety,
                    PersonSurvivalStatType.Satiety,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Hydration,
                    PersonSurvivalStatType.Hydration,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Intoxication,
                    PersonSurvivalStatType.Intoxication,
                    statList);

                CreateStatFromScheme(personScheme.SurvivalStats,
                    SurvivalStatType.Wound,
                    PersonSurvivalStatType.Wound,
                    statList);
            }

            CreateUselessStat(SurvivalStatType.Breath, statList);
            CreateUselessStat(SurvivalStatType.Energy, statList);

            return statList.ToArray();
        }

        private static void CreateUselessStat(SurvivalStatType statType, List<SurvivalStat> statList)
        {
            var stat = new SurvivalStat(100, 0, 100)
            {
                Type = statType,
                Rate = 1,
                DownPassRoll = 0
            };

            statList.Add(stat);
        }

        private void Stat_Changed(object sender, EventArgs e)
        {
            DoStatChanged((SurvivalStat)sender);
        }

        private static void CreateStatFromScheme(IPersonSurvivalStatSubScheme[] survivalStats,
            SurvivalStatType statType,
            PersonSurvivalStatType schemeStatType,
            List<SurvivalStat> statList)
        {
            var stat = CreateStat(statType, schemeStatType, survivalStats);
            if (stat != null)
            {
                statList.Add(stat);
            }
        }

        private static void SetHitPointsStat(IPersonScheme personScheme, IList<SurvivalStat> statList)
        {
            var hpStat = new HpSurvivalStat(personScheme.Hp, 0, personScheme.Hp)
            {
                Type = SurvivalStatType.Health
            };

            statList.Add(hpStat);
        }

        /// <summary>Обновление состояния данных о выживании.</summary>
        public override void Update()
        {
            foreach (var stat in Stats)
            {
                if (stat.Rate == 0)
                {
                    continue;
                }

                var roll = _randomSource.RollSurvival(stat);
                var statDownRoll = GetStatDownRoll(stat);

                if (roll < statDownRoll)
                {
                    ChangeStatInner(stat, -stat.Rate);
                }
            }
        }

        private static SurvivalStat CreateStat(
            SurvivalStatType type,
            PersonSurvivalStatType schemeStatType,
            IPersonSurvivalStatSubScheme[] survivalStats)
        {
            var statScheme = survivalStats.SingleOrDefault(x => x.Type == schemeStatType);
            if (statScheme is null)
            {
                return null;
            }

            var keySegmentList = new List<SurvivalStatKeySegment>();
            if (statScheme.KeyPoints != null)
            {
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Max, PersonSurvivalStatKeypointLevel.Max, statScheme.KeyPoints, keySegmentList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Strong, PersonSurvivalStatKeypointLevel.Strong, statScheme.KeyPoints, keySegmentList);
                AddKeyPointFromScheme(SurvivalStatHazardLevel.Lesser, PersonSurvivalStatKeypointLevel.Lesser, statScheme.KeyPoints, keySegmentList);
            }

            var stat = new SurvivalStat(statScheme.StartValue, statScheme.MinValue, statScheme.MaxValue)
            {
                Type = type,
                Rate = 1,
                KeySegments = keySegmentList.ToArray(),
                DownPassRoll = statScheme.DownPassRoll.GetValueOrDefault(SurvivalStat.DEFAULT_DOWN_PASS_VALUE)
            };

            return stat;
        }

        private static void AddKeyPointFromScheme(
            SurvivalStatHazardLevel segmentLevel,
            PersonSurvivalStatKeypointLevel schemeSegmentLevel,
            IPersonSurvivalStatKeySegmentSubScheme[] keyPoints,
            List<SurvivalStatKeySegment> keyPointList)
        {
            var schemeKeySegment = GetKeyPointSchemeValue(schemeSegmentLevel, keyPoints);
            if (schemeKeySegment == null)
            {
                return;
            }

            var keySegment = new SurvivalStatKeySegment(schemeKeySegment.Start, schemeKeySegment.End, segmentLevel);
            keyPointList.Add(keySegment);
        }

        private void DoStatChanged(SurvivalStat stat)
        {
            var args = new SurvivalStatChangedEventArgs(stat);
            InvokeStatChangedEvent(this, args);
        }

        private static int GetStatDownRoll(SurvivalStat stat)
        {
            return stat.DownPassRoll;
        }

        /// <summary>Сброс всех характеристик к первоначальному состоянию.</summary>
        public override void ResetStats()
        {
            Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health)?.ChangeStatRange(0, _personScheme.Hp);

            foreach (var stat in Stats)
            {
                stat.DownPassRoll = SurvivalStat.DEFAULT_DOWN_PASS_VALUE;
            }
        }

        private static IPersonSurvivalStatKeySegmentSubScheme GetKeyPointSchemeValue(
            PersonSurvivalStatKeypointLevel level,
            IPersonSurvivalStatKeySegmentSubScheme[] keyPoints)
        {
            return keyPoints.SingleOrDefault(x => x.Level == level);
        }
    }
}