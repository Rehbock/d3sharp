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

using System;
using System.Reflection;
using System.Threading;
using D3Sharp.Net.BNet;
using D3Sharp.Net.Game;
using D3Sharp.Utils;

namespace D3Sharp
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private static BnetServer _bnetServer;
        private static GameServer _gameServer;

        public static void Main(string[] args)
        {
            // Watch for unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            
            // Don't forget this..
            LogManager.Enabled = true;
            LogManager.AttachLogTarget(new ConsoleTarget(Level.Trace));
            LogManager.AttachLogTarget(new FileTarget(Level.Trace, "d3sharp-log.txt"));

            PrintBanner();
            PrintLicense();

            Logger.Info("D3Sharp v{0} warming-up..", Assembly.GetExecutingAssembly().GetName().Version);
            StartupServers();
        }

        private static void StartupServers()
        {
            _bnetServer = new BnetServer();
            _gameServer = new GameServer();

            var bnetServerThread = new Thread(_bnetServer.Run) { IsBackground = true };
            bnetServerThread.Start();

            var gameServerThread = new Thread(_gameServer.Run) { IsBackground = true };
            gameServerThread.Start();

            // Read user input indefinitely
            // TODO: Replace with proper command parsing and execution
            while (true)
            {
                var line = Console.ReadLine();
                if (!string.Equals("quit", line, StringComparison.OrdinalIgnoreCase)
                 && !string.Equals("exit", line, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                Logger.Info("Shutting down servers...");
                _bnetServer.Shutdown();
                _gameServer.Shutdown();
                break;
            }
        }

        private static void PrintBanner()
        {
            Console.WriteLine(@"      _  _____       _                         ");
            Console.WriteLine(@"   __| ||___ /  ___ | |__    __ _  _ __  _ __  ");
            Console.WriteLine(@"  / _` |  |_ \ / __|| '_ \  / _` || '__|| '_ \ ");
            Console.WriteLine(@" | (_| | ___) |\__ \| | | || (_| || |   | |_) |");
            Console.WriteLine(@"  \__,_||____/ |___/|_| |_| \__,_||_|   | .__/ ");
            Console.WriteLine(@"                                        |_|    ");
            Console.WriteLine();
        }

        private static void PrintLicense()
        {
            Console.WriteLine("Copyright (C) 2011 D3Sharp Project");
            Console.WriteLine("D3Sharp comes with ABSOLUTELY NO WARRANTY.");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under certain conditions; see the LICENSE file for details.");
            Console.WriteLine();
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
                Logger.FatalException((e.ExceptionObject as Exception), "Application terminating because of unhandled exception.");
            else
                Logger.ErrorException((e.ExceptionObject as Exception), "Caught unhandled exception.");
            Console.ReadLine();
        }
    }
}
