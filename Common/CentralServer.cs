﻿using System;
using System.Collections.Generic;
using log4net;
using log4net.Config;
using NineToFive.Constants;
using NineToFive.Net;

[assembly: XmlConfigurator(ConfigFile = "central-logger.xml")]

namespace NineToFive {
    public class CentralServer {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CentralServer));
        public static readonly Dictionary<string, Client> Clients = new Dictionary<string, Client>(StringComparer.OrdinalIgnoreCase);

        static void Main(string[] args) {
            Log.Info("Hello World, from Central Server!");
            Server.Initialize();
            Interoperability.ServerCreate(ServerConstants.InterCentralPort);
            Log.Info($"Interoperability listening on port {ServerConstants.InterCentralPort}");
        }
    }
}