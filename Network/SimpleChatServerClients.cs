using CustomTCPServerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SimpleChat.Network
{
    public class SimpleChatServerClients
    {
        private List<(CustomTCPServerClient client, string name)> _ClientsCollection { get; } = new();

        public IEnumerable<(CustomTCPServerClient client, string name)> Clients
        {
            get
            {
                lock (_LockObject)
                {
                    return _ClientsCollection;
                }
            }
        }

        private object _LockObject { get; } = new();

        public string[] GetClientNames()
        {
            lock (_LockObject)
            {
                return _ClientsCollection.Select(u => u.name).ToArray();
            }
        }

        public (CustomTCPServerClient client, string name) AppendClient(CustomTCPServerClient client, string name)
        {
            lock (_LockObject)
            {
                if (_ClientsCollection.FirstOrDefault(u => u.client == client).client == null)
                {
                    _ClientsCollection.Add(new(client, name));
                }

                return _ClientsCollection.Last();
            }
        }

        public (CustomTCPServerClient client, string name) RemoveClient(CustomTCPServerClient client)
        {
            var _client = _ClientsCollection.FirstOrDefault(u => u.client == client);

            if (_client.client != null)
            {
                _ClientsCollection.Remove(_client);
            }

            return _client;
        }

        public CustomTCPServerClient? GetClientByName(string? name)
        {
            var client = _ClientsCollection.FirstOrDefault(u => u.name == name);

            return client.client;
        }

        internal void Clear()
        {
            lock (_LockObject)
            {
                _ClientsCollection.Clear();
            }
        }
    }
}
