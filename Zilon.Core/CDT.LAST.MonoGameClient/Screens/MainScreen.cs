﻿using System;
using System.Linq;

using CDT.LAST.MonoGameClient.ViewModels.MainScene;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace CDT.LAST.MonoGameClient.Screens
{
    internal class MainScreen : GameSceneBase
    {
        private readonly Camera _camera;
        private readonly PersonConditionsPanel _personEffectsPanel;
        private readonly IPlayer _player;
        private readonly SpriteBatch _spriteBatch;
        private readonly ITransitionPool _transitionPool;
        private readonly ISectorUiState _uiState;
        private ISector? _currentSector;

        private bool _isTransitionPerforming;

        private SectorViewModel? _sectorViewModel;

        public MainScreen(Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;

            var serviceScope = ((LivGame)Game).ServiceProvider;

            _uiState = serviceScope.GetRequiredService<ISectorUiState>();
            _player = serviceScope.GetRequiredService<IPlayer>();
            _transitionPool = serviceScope.GetRequiredService<ITransitionPool>();

            _camera = new Camera();
            _personEffectsPanel = new PersonConditionsPanel(game, _uiState, screenX: 0, screenY: 0);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (_sectorViewModel != null)
            {
                _sectorViewModel.Draw(gameTime);
            }

            DrawHud();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_sectorViewModel is null)
            {
                _sectorViewModel = new SectorViewModel(Game, _camera, _spriteBatch);
                _currentSector = _sectorViewModel.Sector;
            }

            _sectorViewModel.Update(gameTime);

            if (_player.MainPerson is null)
            {
                throw new InvalidOperationException();
            }

            var isInTransition = _transitionPool.CheckPersonInTransition(_player.MainPerson);

            if (_uiState.ActiveActor != null && !isInTransition)
            {
                var sectorNode = GetPlayerSectorNode(_player);

                if (sectorNode != null)
                {
                    if (_currentSector == sectorNode.Sector)
                    {
                        _camera.Follow(_uiState.ActiveActor, Game);
                        _personEffectsPanel.Update();
                    }
                    else if (!_isTransitionPerforming)
                    {
                        _isTransitionPerforming = true;
                        TargetScene = new TransitionScreen(Game, _spriteBatch);
                    }
                }
            }
            else
            {
                if (!_isTransitionPerforming)
                {
                    _isTransitionPerforming = true;
                    TargetScene = new TransitionScreen(Game, _spriteBatch);
                }
            }
        }

        private void DrawHud()
        {
            _spriteBatch.Begin();
            _personEffectsPanel.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        private static ISectorNode? GetPlayerSectorNode(IPlayer player)
        {
            if (player.Globe is null)
            {
                throw new InvalidOperationException();
            }

            return (from sectorNode in player.Globe.SectorNodes
                    let sector = sectorNode.Sector
                    where sector != null
                    from actor in sector.ActorManager.Items
                    where actor.Person == player.MainPerson
                    select sectorNode).SingleOrDefault();
        }
    }
}