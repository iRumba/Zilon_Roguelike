﻿using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Zilon.Bot.Sdk;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.World;

namespace Zilon.Emulation.Common
{
    public abstract class AutoplayEngineBase<T> where T : IPluggableActorTaskSource<ISectorTaskSourceContext>
    {
        private const int ITERATION_LIMIT = 40_000_000;
        private readonly IGlobeInitializer _globeInitializer;

        protected IServiceScope ServiceScope { get; set; }

        protected BotSettings BotSettings { get; }

        protected AutoplayEngineBase(BotSettings botSettings,
            IGlobeInitializer globeInitializer)
        {
            BotSettings = botSettings;
            _globeInitializer = globeInitializer;
        }

        public async Task<IGlobe> CreateGlobeAsync()
        {
            // Create globe
            var globeInitializer = _globeInitializer;
            var globe = await globeInitializer.CreateGlobeAsync("intro").ConfigureAwait(false);
            return globe;
        }

        public async Task StartAsync(IGlobe globe, IPerson followedPerson)
        {
            var iterationCounter = 1;
            while (!followedPerson.GetModule<ISurvivalModule>().IsDead && iterationCounter <= ITERATION_LIMIT)
            {
                try
                {
                    await globe.UpdateAsync().ConfigureAwait(false);
                }
                catch (ActorTaskExecutionException exception)
                {
                    CatchActorTaskExecutionException(exception);
                }
                catch (AggregateException exception)
                {
                    CatchException(exception.InnerException);
                    throw;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception exception)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    CatchException(exception);
                    throw;
                }
            }

            ProcessEnd();
        }

        protected abstract void CatchException(Exception exception);

        protected abstract void CatchActorTaskExecutionException(ActorTaskExecutionException exception);

        protected abstract void ProcessEnd();

        protected abstract void ConfigBotAux();

        protected abstract void ProcessSectorExit();
    }
}
