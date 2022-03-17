using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jindium
{
    public class SessionData : IEnumerable<Dictionary<string, string>>
    {
        private readonly List<Dictionary<string, string>> _rows = new List<Dictionary<string, string>>();

        public void Add(Dictionary<string, string> href)
        {
            _rows.Add(href);
        }

        public void AddRange(IEnumerable<Dictionary<string, string>> hrefs)
        {
            _rows.AddRange(hrefs);
        }

        public IEnumerator<Dictionary<string, string>> GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_rows).GetEnumerator();
        }
    }

    public class Sessions
    {
        public Dictionary<string, SessionData> SessionsData { get; private set; } = new Dictionary<string, SessionData>();

        private string GenerateSessionId()
        {
            return Guid.NewGuid().ToString();
        }

        public string Add(Dictionary<string, string> href)
        {
            string sessionId = GenerateSessionId();

            if (!SessionsData.ContainsKey(sessionId))
            {
                SessionsData.Add(sessionId, new SessionData());
            }

            SessionsData[sessionId].Add(href);

            return sessionId;
        }
    }
}
