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
using System.IO;
using System.Net;
using System.Text;
using AustinHarris.JsonRpc;
using Coinium.Common.Extensions;
using Coinium.Core.Mining;
using Coinium.Core.Mining.Events;
using Coinium.Core.Mining.Miner;
using Coinium.Core.RPC.Http;
using Coinium.Core.Server.Stratum.Notifications;
using Serilog;

namespace Coinium.Core.Server.Vanilla
{
    public class VanillaMiner : IMiner
    {
        /// <summary>
        /// Unique subscription id for identifying the miner.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Is the miner authenticated?
        /// </summary>
        public bool Authenticated { get; private set; }

        /// <summary>
        /// Event that fires when a miner authenticates.
        /// </summary>
        public event EventHandler OnAuthenticate;

        /// <summary>
        /// Can we send new mining job's to miner?
        /// </summary>
        public bool SupportsJobNotifications { get; private set; }

        public VanillaMiner(int id)
        {
            this.Id = id; // the id of the miner.
            this.SupportsJobNotifications = false; // vanilla miner's doesn't support new mining job notifications.
            this.Authenticated = false;
        }

        public void Parse(HttpListenerContext httpContext)
        {
            var httpRequest = httpContext.Request;

            var encoding = Encoding.UTF8;

            var rpcResultHandler = new AsyncCallback(
                callback =>
                {
                    var asyncData = ((JsonRpcStateAsync) callback);

                    var result = asyncData.Result;
                    var data = Encoding.UTF8.GetBytes(result);
                    var request = ((HttpRpcResponse) asyncData.AsyncState);

                    request.Response.ContentType = "application/json";
                    request.Response.ContentEncoding = encoding;

                    request.Response.ContentLength64 = data.Length;
                    request.Response.OutputStream.Write(data,0,data.Length);                    
                });

            using (var reader = new StreamReader(httpRequest.InputStream, encoding))
            {
                var line = reader.ReadToEnd();            
                Log.Verbose(line.PretifyJson());
                var response = httpContext.Response;

                var rpcResponse = new HttpRpcResponse(line, response);
                var rpcContext = new HttpRpcContext(this, rpcResponse);

                var async = new JsonRpcStateAsync(rpcResultHandler, rpcResponse) { JsonRpc = line };
                JsonRpcProcessor.Process(async, rpcContext);
            }        
        }

        public bool Authenticate(string user, string password)
        {
            this.Authenticated = true;

            // notify any listeners about the miner's authentication.
            var handler = OnAuthenticate;
            if (handler != null)
                handler(this, new MinerAuthenticationEventArgs(this));


            return this.Authenticated;
        }

        /// <summary>
        /// Sends difficulty to the miner.
        /// </summary>
        /// <param name="difficulty"></param>
        public void SendDifficulty(Difficulty difficulty)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sends a new mining job to the miner.
        /// </summary>
        public void SendJob(Job job)
        {
            throw new NotSupportedException();
        }
    }
}
