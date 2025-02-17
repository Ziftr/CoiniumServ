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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Coinium.Common.Extensions;
using Coinium.Core.Coin.Transactions;
using Coinium.Core.Crypto;
using Coinium.Core.Mining.Pool;
using Coinium.Core.Server.Stratum.Notifications;

namespace Coinium.Core.Mining.Jobs
{
    public class JobManager : IJobManager
    {
        private readonly Dictionary<UInt64, Job> _jobs = new Dictionary<UInt64, Job>();
        private readonly JobCounter _jobCounter = new JobCounter();
        private IExtraNonce _extraNonce;

        public IPool Pool { get; set; }

        public IExtraNonce ExtraNonce { get { return this._extraNonce; } }
        public Job LastJob { get; private set; }

        public JobManager()
        {
        }

        public void Initialize(IPool pool, ulong instanceId)
        {
            Pool = pool;
            _extraNonce = new ExtraNonce(instanceId);
        }

        public Job GetJob(UInt64 id)
        {
            return this._jobs.ContainsKey(id) ? this._jobs[id] : null;
        }

        public void AddJob(Job job)
        {
            this._jobs.Add(job.Id, job);
        }

        /// <summary>
        /// Broadcasts to miners.
        /// </summary>
        /// <example>
        /// sample communication: http://bitcoin.stackexchange.com/a/23112/8899
        /// </example>
        /// <param name="state"></param>
        public void Broadcast()
        {
            var blockTemplate = this.Pool.DaemonClient.GetBlockTemplate();
            var generationTransaction = new GenerationTransaction(this.Pool, blockTemplate, false);

            var hashList = new List<byte[]>();

            foreach (var transaction in blockTemplate.Transactions)
            {
                hashList.Add(transaction.Hash.HexToByteArray());
            }

            var merkleTree = new MerkleTree(hashList);


            // create the difficulty notification.
            var difficulty = new Difficulty(16);

            // create the job notification.
            var job = new Job(this._jobCounter.Next(), blockTemplate, generationTransaction, merkleTree)
            {
                CleanJobs = true // tell the miners to clean their existing jobs and start working on new one.
            };

            this._jobs.Add(job.Id,job);
            this.LastJob = job;

            foreach (var miner in this.Pool.MinerManager.GetAll())
            {
                if (!miner.SupportsJobNotifications)
                    continue;

                miner.SendDifficulty(difficulty);
                miner.SendJob(job);
            }
        }
    }
}
