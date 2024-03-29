﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium
{
    public class SessionData : Dictionary<string, string>
    {
        public void AddKeyValue(string key, string value)
        {
            if (ContainsKey(key))
                this[key] = value;
            else
                Add(key, value);
        }

        public string GetValue(string key)
        {
            return ContainsKey(key) ? this[key] : "";
        }

        public bool IsAuthenticated { get; set; } = false;
    }

    public class Sessions
    {
        public bool SessionsActive { get; set; } = true;
        public Dictionary<string, SessionData> SessionsData { get; private set; } = new Dictionary<string, SessionData>();

        private string GenerateSessionId()
        {
            return Utilities.Sha256Hash(Guid.NewGuid().ToString());
        }

        public string StartSession()
        {
            string sessionId = GenerateSessionId();
            if (!SessionsData.ContainsKey(sessionId)) SessionsData.Add(sessionId, new SessionData());
            return sessionId;
        }

        public SessionData GetSession(string sessionId)
        {
            if (SessionsData.ContainsKey(sessionId)) return SessionsData[sessionId];
            return new SessionData();
        }
    }
}
