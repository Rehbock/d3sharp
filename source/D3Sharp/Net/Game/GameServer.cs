/*
 * Copyright (C) 2011 D3Sharp Project
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System.Threading;
using System.Collections.Generic;
using D3Sharp.Utils.Extensions;
using D3Sharp.Core.Universe;

namespace D3Sharp.Net.Game
{
    public sealed class GameServer : Server
    {
        Universe GameUniverse;

        List<GameClient> clients = new List<GameClient>();
        Thread thread, mainThread;

        public GameServer()
        {
            GameUniverse = new Universe();

            this.OnConnect += GameServer_OnConnect;
            this.OnDisconnect += (sender, e) => Logger.Trace("Client disconnected: {0}", e.Connection.ToString());
            this.DataReceived += GameServer_DataReceived;
            this.DataSent += (sender, e) => { };

            mainThread = Thread.CurrentThread;

            thread = new Thread(new ThreadStart(() => { GameServer_Thread(); }));
            thread.Start();
        }

        void GameServer_Thread()
        {
            while (mainThread.IsAlive)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].GameUniverse != null &&  clients[i].GameUniverse.GetLocalWorld() != null && clients[i].GameUniverse.GetLocalWorld().InWorld())
                    {
                        lock (clients[i].GameUniverse)
                        {
                            clients[i].GameUniverse.Tick(clients[i]);
                        }
                    }
                }
                Thread.Sleep(1);
            }
        }

        void GameServer_DataReceived(object sender, ConnectionDataEventArgs e)
        {
            var connection = (Connection)e.Connection;
            thread.Suspend();
            ((GameClient)connection.Client).Parse(e);
            thread.Resume();
        }

        void GameServer_OnConnect(object sender, ConnectionEventArgs e)
        {
            Logger.Trace("Game-Client connected: {0}", e.Connection.ToString());
            e.Connection.Client = new GameClient(e.Connection, GameUniverse);

            thread.Suspend();
            clients.Add((GameClient)e.Connection.Client);
            thread.Resume();
        }

        public override void Run()
        {
            // we can't listen for port 1119 because D3 and the launcher (agent) communicates on that port through loopback.
            // so we change our default port and start D3 with a shortcut like so:
            //   "F:\Diablo III Beta\Diablo III.exe" -launch -auroraaddress 127.0.0.1:1345

            if (!this.Listen(Config.Instance.BindIP, Config.Instance.Port)) return;
            Logger.Info("Game-Server is listening on {0}:{1}...", Config.Instance.BindIP, Config.Instance.Port);
        }
    }
}