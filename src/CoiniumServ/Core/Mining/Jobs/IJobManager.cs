﻿/*
 *   CoiniumServ - crypto currency pool software - https://github.com/CoiniumServ/CoiniumServ
 *   Copyright (C) 2013 - 2014, Coinium Project - http://www.coinium.org
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Server.Stratum.Notifications;

namespace Coinium.Core.Mining.Jobs
{
    public interface IJobManager
    {
        IPool Pool { get; set; }

        IExtraNonce ExtraNonce { get; }

        Job GetJob(UInt64 id);

        void AddJob(Job job);

        void Broadcast();

        /// <summary>
        /// Initializes the specified pool.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="instanceId">The instance identifier.</param>
        void Initialize(IPool pool, ulong instanceId);
    }
}
