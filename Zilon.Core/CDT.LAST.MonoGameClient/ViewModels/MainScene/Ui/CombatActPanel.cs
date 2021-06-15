﻿using System;
using System.Collections.Generic;
using System.Linq;

using CDT.LAST.MonoGameClient.Engine;
using CDT.LAST.MonoGameClient.Screens;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene.Ui
{
    public sealed class CombatActPanel
    {
        private const int MAX_COMBAT_ACT_COUNT = 8;
        private readonly CombatActButtonGroup _buttonGroup;
        private readonly IList<CombatActButton> _buttons;
        private readonly ICombatActModule _combatActModule;
        private readonly IEquipmentModule _equipmentModule;
        private readonly ISectorUiState _sectorUiState;
        private readonly IUiContentStorage _uiContentStorage;
        private readonly IconButton _idleModeSwitcherButton;

        public CombatActPanel(ICombatActModule combatActModule, IEquipmentModule equipmentModule,
            IUiContentStorage uiContentStorage, ISectorUiState sectorUiState)
        {
            _combatActModule = combatActModule;
            _equipmentModule = equipmentModule;
            _uiContentStorage = uiContentStorage;
            _sectorUiState = sectorUiState;

            _buttons = new List<CombatActButton>();

            _buttonGroup = new CombatActButtonGroup();

            Initialize(_buttons);

            _idleModeSwitcherButton = new IconButton(uiContentStorage.GetSmallVerticalButtonBackgroundTexture(),
                new IconData(uiContentStorage.GetSmallVerticalButtonIconsTexture(), new Rectangle(0, 0, 16, 32)),
                new Rectangle(0, 0, 16, 32));
            _idleModeSwitcherButton.OnClick += IdleModeSwitcherButton_OnClick;

            _equipmentModule.EquipmentChanged += EquipmentModule_EquipmentChanged;
        }

        private void IdleModeSwitcherButton_OnClick(object? sender, EventArgs e)
        {
            _combatActModule.IsCombatMode = !_combatActModule.IsCombatMode;
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            const int BUTTON_SIZE = 32;
            const int BOTTOM_MARGIN = 0;

            var acts = _combatActModule.CalcCombatActs().Take(MAX_COMBAT_ACT_COUNT);
            var actsOrdered = acts.OrderBy(x => x.Scheme?.Sid).ToArray();
            var actIndex = 0;
            foreach (var button in _buttons)
            {
                var buttonRect = new Rectangle(
                    actIndex * BUTTON_SIZE + graphicsDevice.Viewport.Width / 2 - BUTTON_SIZE * MAX_COMBAT_ACT_COUNT / 2,
                    graphicsDevice.Viewport.Bounds.Bottom - BUTTON_SIZE - BOTTOM_MARGIN, BUTTON_SIZE,
                    BUTTON_SIZE);

                button.Rect = buttonRect;

                button.Draw(spriteBatch);

                actIndex++;
            }

            _idleModeSwitcherButton.Rect = new Rectangle(
                    actIndex * BUTTON_SIZE + graphicsDevice.Viewport.Width / 2 - BUTTON_SIZE * MAX_COMBAT_ACT_COUNT / 2,
                    graphicsDevice.Viewport.Bounds.Bottom - BUTTON_SIZE - BOTTOM_MARGIN, 16,
                    BUTTON_SIZE);
            _idleModeSwitcherButton.Draw(spriteBatch);
        }

        public void UnsubscribeEvents()
        {
            _equipmentModule.EquipmentChanged -= EquipmentModule_EquipmentChanged;
        }

        public void Update()
        {
            foreach (var button in _buttons)
            {
                button.Update();
            }

            _idleModeSwitcherButton.Update();
        }

        private void EquipmentModule_EquipmentChanged(object? sender, EquipmentChangedEventArgs e)
        {
            _buttons.Clear();
            Initialize(_buttons);
        }

        private void Initialize(IList<CombatActButton> _buttons)
        {
            var acts = _combatActModule.CalcCombatActs();
            var actsOrdered = acts.OrderBy(x => x.Scheme?.Sid).ToArray();
            foreach (var act in actsOrdered)
            {
                const int BUTTON_SIZE = 32;
                var tags = act.Scheme?.Stats?.Tags?.Where(x => x != null)?.Select(x => x!)?.ToArray() ??
                           Array.Empty<string>();
                var button = new CombatActButton(_uiContentStorage.GetButtonTexture(),
                    _uiContentStorage.GetCombatActIconTexture(act.Scheme?.Sid, tags),
                    _uiContentStorage.GetHintBackgroundTexture(),
                    _buttonGroup,
                    new Rectangle(0, 0, BUTTON_SIZE, BUTTON_SIZE));

                button.OnClick += (s, e) =>
                {
                    _sectorUiState.TacticalAct = act;
                    _buttonGroup.Selected = (CombatActButton?)s;
                };

                if (act == _sectorUiState.TacticalAct)
                {
                    _buttonGroup.Selected = button;
                }

                _buttons.Add(button);
            }
        }
    }
}