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
using System.Net;
using System.Net.Sockets;

namespace Coinium.Net.Server.Sockets
{
    public class Connection : IConnection
    {
        /// <summary>
        /// Gets or sets bound client.
        /// </summary>
        public IClient Client { get; set; }

        /// <summary>
        /// Gets underlying socket.
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// Returns true if there exists an active connection.
        /// </summary>
        public bool IsConnected
        {
            get { return (Socket != null) && Socket.Connected; }
        }

        /// <summary>
        /// Returns remote endpoint.
        /// </summary>
        public IPEndPoint RemoteEndPoint
        {
            get { return (Socket == null) ? null : Socket.RemoteEndPoint as IPEndPoint; }
        }

        /// <summary>
        /// Returns local endpoint.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get { return Socket.LocalEndPoint as IPEndPoint; }
        }

        /// <summary>
        /// The server instance that connection is bound to.
        /// </summary>
        private SocketServer _server;

        /// <summary>
        /// Default buffer size.
        /// </summary>
        public static readonly int BufferSize = 16 * 1024; // 16 KB   

        /// <summary>
        /// The recieve buffer.
        /// </summary>
        private readonly byte[] _recvBuffer = new byte[BufferSize];

        /// <summary>
        /// Returns the recieve-buffer.
        /// </summary>
        public byte[] RecvBuffer
        {
            get { return _recvBuffer; }
        }

        public Connection(SocketServer server, Socket socket)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            if (socket == null)
                throw new ArgumentNullException("socket");

            this._server = server;
            this.Socket = socket;
        }

        #region recieve methods

        // Read bytes from the Socket into the buffer in a non-blocking call.
        // This allows us to read no more than the specified count number of bytes.\
        // Note that this method should only be called prior to encryption!
        public int Receive(int start, int count)
        {
            return this.Socket.Receive(_recvBuffer, start, count, SocketFlags.None);
        }

        /// <summary>
        /// Begins recieving data async.
        /// </summary>
        /// <param name="callback">Callback function be called when recv() is complete.</param>
        /// <param name="state">State manager object.</param>
        /// <returns>Returns <see cref="IAsyncResult"/></returns>
        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.Socket.BeginReceive(_recvBuffer, 0, BufferSize, SocketFlags.None, callback, state);
        }

        public int EndReceive(IAsyncResult result)
        {
            return this.Socket.EndReceive(result);
        }

        #endregion

        #region send methods

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(byte[] buffer)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            return Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        /// <summary>
        /// Sends an enumarable byte buffer to remote endpoint.
        /// </summary>
        /// <param name="data">Enumrable byte buffer to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(IEnumerable<byte> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            return Send(data, SocketFlags.None);
        }

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(byte[] buffer, SocketFlags flags)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            return Send(buffer, 0, buffer.Length, flags);
        }

        /// <summary>
        /// Sends an enumarable byte buffer to remote endpoint.
        /// </summary>
        /// <param name="data">Enumrable byte buffer to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(IEnumerable<byte> data, SocketFlags flags)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (_server == null)
            {
                throw new Exception("[Connection] _server is null in Send");
            }
            return _server.Send(this, data, flags);
        }

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="start">Start index to read from buffer.</param>
        /// <param name="count">Count of bytes to send.</param>
        /// <returns>Returns count of sent bytes.</returns>
        public int Send(byte[] buffer, int start, int count)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            return Send(buffer, start, count, SocketFlags.None);
        }

        /// <summary>
        /// Sends byte buffer to remote endpoint.
        /// </summary>
        /// <param name="buffer">Byte buffer to send.</param>
        /// <param name="start">Start index to read from buffer.</param>
        /// <param name="count">Count of bytes to send.</param>
        /// <param name="flags">Sockets flags to use.</param>
        /// <returns
        public int Send(byte[] buffer, int start, int count, SocketFlags flags)
        {
            if (buffer == null) 
                throw new ArgumentNullException("buffer");

            if (_server == null)            
                throw new Exception("Connection is not bound to a server instance.");

            return _server.Send(this, buffer, start, count, flags);
        }

        #endregion

        #region disconnect methods

        public void Disconnect()
        {
            if(Socket.Connected)
                this.Socket.Disconnect(true);

            this.Client = null;            
        }

        #endregion

        /// <summary>
        /// Returns a connection state string.
        /// </summary>
        /// <returns>Connection state string.</returns>
        public override string ToString()
        {
            if (Socket == null)
                return "No Socket!";
            else
                return Socket.RemoteEndPoint != null ? Socket.RemoteEndPoint.ToString() : "Not Connected!";
        }
    }
}
