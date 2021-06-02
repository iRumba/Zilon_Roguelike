﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CDT.LAST.MonoGameClient.Resources;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Zilon.Core.Client;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal class PersonConditionsPanel
    {
        private const int ICON_SIZE = 16;
        private const int ICON_SPACING = 2;

        private readonly Texture2D _conditionBackgroundTexture;
        private readonly Texture2D _conditionHintBackgroundTexture;
        private readonly Dictionary<string, Texture2D> _conditionIconTextureDict;
        private readonly SpriteFont _hintTitleFont;
        private readonly int _screenX;
        private readonly int _screenY;
        private readonly ISectorUiState _uiState;
        private IPersonCondition? _selectedCondition;
        private int? _selectedConditionIconIndex;

        public PersonConditionsPanel(Game game, ISectorUiState uiState, int screenX, int screenY)
        {
            _uiState = uiState;
            _screenX = screenX;
            _screenY = screenY;
            _conditionBackgroundTexture =
                game.Content.Load<Texture2D>("Sprites/ui/PersonConditions/ConditionIconBackground");

            var conditionIconTextureSids = new[]
            {
                "HungerLesser",
                "HungerStrong",
                "HungerCritical",

                "ThristLesser",
                "ThristStrong",
                "ThristCritical",

                "IntoxicationLesser",
                "IntoxicationStrong",
                "IntoxicationCritical",

                "WoundLesser",
                "WoundStrong",
                "WoundCritical",

                "DiseaseSymptom"
            };
            _conditionIconTextureDict = conditionIconTextureSids.ToDictionary(
                sid => sid,
                sid => game.Content.Load<Texture2D>($"Sprites/ui/PersonConditions/{sid}ConditionIcon"));
            _conditionHintBackgroundTexture =
                game.Content.Load<Texture2D>("Sprites/ui/PersonConditions/ConditionHintBackground");
            _hintTitleFont = game.Content.Load<SpriteFont>("Fonts/HintTitle");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                return;
            }

            var conditionsModule = person.GetModule<IConditionsModule>();

            var conditionIndex = 0;
            foreach (var condition in conditionsModule.Items)
            {
                var iconX = conditionIndex * (ICON_SIZE + ICON_SPACING) + _screenX;

                DrawIconBackground(spriteBatch, iconX);

                DrawIcon(spriteBatch, condition, iconX);

                conditionIndex++;
            }

            DrawHintIfSelected(spriteBatch);
        }

        public void Update()
        {
            var person = _uiState.ActiveActor?.Actor?.Person;
            if (person is null)
            {
                return;
            }

            var conditionsModule = person.GetModule<IConditionsModule>();

            var mouseState = Mouse.GetState();

            var effectRectangles = conditionsModule.Items.Select((x, index) => new
            {
                UiRect = new Rectangle(index * (ICON_SIZE + ICON_SPACING) - ICON_SPACING + _screenX, _screenY,
                    ICON_SIZE, ICON_SIZE),
                Condition = x,
                IconIndex = index
            });
            var mouseRectangle = new Rectangle(mouseState.X, mouseState.Y, 1, 1);

            var effectUnderMouse = effectRectangles.FirstOrDefault(x => x.UiRect.Intersects(mouseRectangle));

            if (effectUnderMouse != null)
            {
                _selectedCondition = effectUnderMouse.Condition;
                _selectedConditionIconIndex = effectUnderMouse.IconIndex;
            }
            else
            {
                _selectedCondition = null;
                _selectedConditionIconIndex = null;
            }
        }

        private void DrawHintIfSelected(SpriteBatch spriteBatch)
        {
            if (_selectedCondition != null && _selectedConditionIconIndex != null)
            {
                var personConditionTitle = GetConditionTitle(_selectedCondition);
                var titleTextSizeVector = _hintTitleFont.MeasureString(personConditionTitle);
                var selectedConditionIndex = _selectedConditionIconIndex.Value;
                var hintXPosition = selectedConditionIndex * (ICON_SIZE + ICON_SPACING) + _screenX;

                const int Y_POSITION_UNDER_ICON_SEQUENCE = ICON_SIZE + ICON_SPACING;
                const int HINT_TEXT_SPACING = 8;
                var hintRectangle = new Rectangle(hintXPosition, Y_POSITION_UNDER_ICON_SEQUENCE + _screenY,
                    (int)titleTextSizeVector.X + HINT_TEXT_SPACING * 2,
                    (int)titleTextSizeVector.Y + HINT_TEXT_SPACING * 2);

                spriteBatch.Draw(_conditionHintBackgroundTexture, hintRectangle, Color.DarkSlateGray);

                spriteBatch.DrawString(_hintTitleFont, personConditionTitle,
                    new Vector2(hintRectangle.Left + HINT_TEXT_SPACING, hintRectangle.Top + HINT_TEXT_SPACING),
                    Color.Wheat);
            }
        }

        private void DrawIcon(SpriteBatch spriteBatch, IPersonCondition effect, int iconX)
        {
            var conditionIconSid = GetConditionSid(effect);
            var conditionIconTexture = _conditionIconTextureDict[conditionIconSid];
            spriteBatch.Draw(conditionIconTexture, new Vector2(iconX, _screenY), Color.DarkSlateGray);
        }

        private void DrawIconBackground(SpriteBatch spriteBatch, int iconX)
        {
            spriteBatch.Draw(_conditionBackgroundTexture, new Rectangle(iconX, _screenY, ICON_SIZE, ICON_SIZE),
                Color.Yellow);
        }

        private static string GetConditionSid(IPersonCondition personCondition)
        {
            switch (personCondition)
            {
                case SurvivalStatHazardCondition statCondition:

                    var typeString = GetStatHazardConditionTypeClientString(statCondition.Type);
                    var levelString = GetStatHazardConditionLevelClientString(statCondition);
                    return $"{typeString}{levelString}";

                case DiseaseSymptomCondition:
                    return "DiseaseSymptom";

                default:
                    Debug.Fail("Every condition must have icon.");
                    return "HungerLesser";
            }
        }

        private static string GetConditionTitle(IPersonCondition personCondition)
        {
            switch (personCondition)
            {
                case SurvivalStatHazardCondition statEffect:
                    return GetSurvivalConditionTitle(statEffect);

                case DiseaseSymptomCondition:
                    return string.Empty;

                default:
                    Debug.Fail(
                        $"All person conditions must have localized titles. Unknown person effect: {personCondition}.");
                    return string.Empty;
            }
        }

        private static string GetStatHazardConditionLevelClientString(SurvivalStatHazardCondition statCondition)
        {
            return statCondition.Level == SurvivalStatHazardLevel.Max
                ? "Critical"
                : statCondition.Level.ToString();
        }

        private static string GetStatHazardConditionTypeClientString(SurvivalStatType type)
        {
            switch (type)
            {
                case SurvivalStatType.Health:
                    return "Wound";

                case SurvivalStatType.Satiety:
                    return "Hunger";

                case SurvivalStatType.Hydration:
                    return "Thrist";

                case SurvivalStatType.Intoxication:
                    return "Intoxication";

                case SurvivalStatType.Undefined:
                    Debug.Fail("Undefined condition.");
                    return "Empty";

                default:
                    Debug.Fail($"Unknown condition type {type}.");
                    return "Empty";
            }
        }

        private static string GetSurvivalConditionTitle(SurvivalStatHazardCondition statCondition)
        {
            var typeString = GetStatHazardConditionTypeClientString(statCondition.Type);
            var levelString = GetStatHazardConditionLevelClientString(statCondition);
            var conditionTitle = UiResources.ResourceManager.GetString($"{typeString}{levelString}ConditionTitle");
            if (string.IsNullOrWhiteSpace(conditionTitle))
            {
                Debug.Fail("All person conditions must have localized titles."
                           + $" Unknown person effect: {statCondition.Type} {statCondition.Level}.");
                return $"{typeString}{levelString}";
            }

            return conditionTitle;
        }
    }
}