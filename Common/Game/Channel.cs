﻿using System;
using System.Collections.Generic;
using System.Net;
using NineToFive.Net;

namespace NineToFive.Game {
    public class Channel {
        private readonly byte _worldId;
        public readonly ChannelSnapshot Snapshot = new ChannelSnapshot();
        public Dictionary<int, Field> Fields { get; } = new Dictionary<int, Field>();

        public Channel(byte worldId, byte id, int port) {
            _worldId = worldId;
            Id = id;
            Port = port;
        }

        public World World => Server.Worlds[_worldId];
        public byte Id { get; }
        public int Port { get; }
        public IPAddress HostAddress { get; set; }
        public ServerListener ServerListener { get; set; }

        public Field GetField(int fieldId) {
            Fields.TryGetValue(fieldId, out Field field);
            if (field != null) return field;
            field = new Field(fieldId);
            Fields.Add(fieldId, field);
            return field;
        }
    }

    /// <summary>
    /// container for data the login server requires from the channel servers
    /// </summary>
    public class ChannelSnapshot {
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Last recorded value of user count 
        /// </summary>
        public int UserCount { get; set; }
    }
}