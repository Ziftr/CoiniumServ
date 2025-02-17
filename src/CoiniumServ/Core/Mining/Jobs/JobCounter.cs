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

namespace Coinium.Core.Mining.Jobs
{
    /// <summary>
    /// Counter for job id's.
    /// </summary>
    public class JobCounter
    {
        private UInt64 Current { get; set; }

        public JobCounter()
        {
            this.Current = 1;
        }

        /// <summary>
        /// Gets a new job id.
        /// </summary>
        /// <returns></returns>
        public UInt64 Next()
        {
            Current++;

            if (Current % 0xffff == 0)
                Current = 1;

            return Current;
        }
    }
}
