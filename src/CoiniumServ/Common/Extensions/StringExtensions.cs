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


using System.IO;
using Jayrock.Json;

namespace Coinium.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Prettifies a json string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string PretifyJson(this string text)
        {
            var input = new StringReader(text);
            var output = new StringWriter();

            using (var reader = new JsonTextReader(input))
            using (var writer = new JsonTextWriter(output))
            {

                writer.PrettyPrint = true;
                writer.WriteFromReader(reader);
            }

            return output.ToString();
        }
    }
}
