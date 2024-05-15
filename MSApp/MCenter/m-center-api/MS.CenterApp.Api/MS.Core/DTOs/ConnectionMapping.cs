using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS.Core.DTOs
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        private readonly Dictionary<string, HashSet<string>> _connectionsByOrg =
           new Dictionary<string, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        /// <summary>
        /// Thêm connectionId theo từng đơn vị
        /// </summary>
        /// <param name="key">key - id đơn vị</param>
        /// <param name="connectionId">connectionId</param>
        public void AddToConnectionsByOrg(string key, string connectionId)
        {
            lock (_connectionsByOrg)
            {
                HashSet<string> connectionsByOrg;
                if (!_connectionsByOrg.TryGetValue(key, out connectionsByOrg))
                {
                    connectionsByOrg = new HashSet<string>();
                    _connectionsByOrg.Add(key, connectionsByOrg);
                }

                lock (connectionsByOrg)
                {
                    connectionsByOrg.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public IEnumerable<string> GetConnectionsByOrgId(string key)
        {
            HashSet<string> connectionsByOrg;
            if (_connectionsByOrg.TryGetValue(key, out connectionsByOrg))
            {
                return connectionsByOrg;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }

        public void RemoveConnectionByOrgId(string key, string connectionId)
        {
            lock (_connectionsByOrg)
            {
                HashSet<string> connectionsByOrg;
                if (!_connectionsByOrg.TryGetValue(key, out connectionsByOrg))
                {
                    return;
                }

                lock (connectionsByOrg)
                {
                    // Xóa connectionId:
                    connectionsByOrg.Remove(connectionId);

                    // Hết rồi thì xóa toàn bộ:
                    if (connectionsByOrg.Count == 0)
                    {
                        _connectionsByOrg.Remove(key);
                    }
                }
            }
        }
        
    }
}
