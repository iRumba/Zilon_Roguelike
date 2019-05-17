﻿using System.Linq;

using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;

namespace Zilon.Bot.Players.Triggers
{
    public class ThirstAndHasResourceTrigger : ILogicStateTrigger
    {
        public ILogicStateData CreateData(IActor actor)
        {
            return new EmptyLogicTriggerData();
        }

        public bool Test(IActor actor, ILogicState currentState, ILogicStateData data)
        {
            var hazardEffect = actor.Person.Effects.Items.OfType<SurvivalStatHazardEffect>()
                .SingleOrDefault(x => x.Type == SurvivalStatType.Water);
            if (hazardEffect == null)
            {
                return false;
            }

            //

            var props = actor.Person.Inventory.CalcActualItems();
            var resources = props.OfType<Resource>();
            var bestResource = ResourceFinder.FindBestConsumableResourceByRule(resources,
                ConsumeCommonRuleType.Thirst);

            if (bestResource == null)
            {
                return false;
            }

            return true;
        }
    }
}
