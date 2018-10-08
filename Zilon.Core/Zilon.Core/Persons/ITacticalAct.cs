﻿using Zilon.Core.Common;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{

    /// <summary>
    /// Интерфейс тактического действия.
    /// </summary>
    public interface ITacticalAct
    {
        /// <summary>
        /// Схема основных характеристик тактического действия.
        /// </summary>
        TacticalActStatsSubScheme Stats { get; }

        /// <summary>
        /// Эффективность действия.
        /// </summary>
        Roll Efficient { get; }
    }
}
