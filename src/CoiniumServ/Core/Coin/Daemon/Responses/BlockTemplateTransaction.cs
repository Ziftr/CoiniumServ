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

namespace Coinium.Core.Coin.Daemon.Responses
{
    /// <summary>
    /// <remarks>
    /// https://github.com/bitcoin/bips/blob/master/bip-0022.mediawiki#Transactions%20Object%20Format
    /// </remarks>
    /// </summary>
    public class BlockTemplateTransaction
    {
        /// <summary>
        /// transaction data encoded in hexadecimal (byte-for-byte)
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// other transactions before this one (by 1-based index in "transactions" list) that must be present in the final block if this one is; 
        /// if key is not present, dependencies are unknown and clients MUST NOT assume there aren't any
        /// </summary>
        public int[] Depends { get; set; }

        /// <summary>
        /// difference in value between transaction inputs and outputs (in Satoshis); for coinbase transactions, this is a negative Number of the 
        /// total collected block fees (ie, not including the block subsidy); if key is not present, fee is unknown and clients MUST NOT assume there isn't one
        /// </summary>
        public int Fee { get; set; }

        /// <summary>
        /// hash/id encoded in little-endian hexadecimal
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// if provided and true, this transaction must be in the final block
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// total number of SigOps, as counted for purposes of block limits; if key is not present, sigop count is unknown and clients MUST NOT assume 
        /// there aren't any
        /// </summary>
        public int Sigops { get; set; }
    }
}
