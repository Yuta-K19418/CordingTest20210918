using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CordingTest20210918
{
    /// <summary>
    /// 監視ログ情報
    /// </summary>
    public class LogInfo
    {
        public DateTime LogTime { get; set; }

        public string ServerAddress { get; set; }

        public int? ReactionMiliSecond { get; set; }

        public string SubnetAddress { get; set; }

        public bool ShouldExclude { get; set; }
    }

    /// <summary>
    /// サーバ情報
    /// </summary>
    public class ServerInfo
    {
        public string ServerAddress { get; set; }

        public bool HasBroken { get; set; }

        public bool IsAggregating { get; set; }

        public int? PingCount { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public TimeSpan? ReactionTime { get; set; }

        public string SubnetAddress { get; set; }
    }

    /// <summary>
    /// サブネット情報
    /// </summary>
    public class SubnetInfo
    {
        public string SubnetAddress { get; set; }

        public List<string> ServerAddressList { get; set; }

        public bool HasBroken { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
