﻿using System;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Players;
using Zilon.Bot.Players.NetCore;
using Zilon.Bot.Players.NetCore.DependencyInjectionExtensions;
using Zilon.Bot.Players.Strategies;
using Zilon.Emulation.Common;

using HumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.HumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IHumanActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IHumanActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;
using IActorTaskSource =
    Zilon.Core.Tactics.Behaviour.IActorTaskSource<Zilon.Core.Tactics.Behaviour.ISectorTaskSourceContext>;

namespace CDT.LIV.MonoGameClient
{
    internal sealed class StartUp : InitializationBase
    {
        public override void ConfigureAux(IServiceProvider serviceFactory)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterBot(IServiceCollection serviceCollection)
        {
            serviceCollection.RegisterLogicState();
            serviceCollection.AddSingleton<ILogicStateFactory>(factory => new ContainerLogicStateFactory(factory));
            serviceCollection.AddSingleton<LogicStateTreePatterns>();

            serviceCollection.AddSingleton<IHumanActorTaskSource, HumanActorTaskSource>();
            serviceCollection.AddSingleton<IActorTaskSource>(provider => provider.GetRequiredService<IHumanActorTaskSource>());
        }
    }
}
