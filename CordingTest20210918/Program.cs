using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CordingTest20210918
{
    public class Program
    {
        public static void Main(string[] args)
        {
            int timeOutCount = Int32.Parse(args[1]);
            int lastNumberOfTimes = Int32.Parse(args[2]);
            int reactionTime = Int32.Parse(args[3]);
            string output = OutputPeriodOfBrokenServer(args[0],
                                                        timeOutCount,
                                                        lastNumberOfTimes,
                                                        reactionTime);
            Console.WriteLine(output);
        }

        private class LogInfo
        {
            public DateTime LogTime { get; set; }

            public string ServerAddress { get; set; }

            public int? ReactionMiliSecond { get; set; }

            public string Subnet { get; set; }
        }

        public static string OutputPeriodOfBrokenServer(string inputFilePath,
                                                        int timeOutCount,
                                                        int lastNumberOfTimes,
                                                        int reactionTime)
        {
            StreamReader sr = new StreamReader(@inputFilePath);
            var startTimeDic = new Dictionary<string, DateTime>();
            var endTimeDic = new Dictionary<string, DateTime>();
            var brokenServerDic = new Dictionary<string, TimeSpan>();
            var pingCountDic = new Dictionary<string, int>();
            var readPinglist = new List<LogInfo>();
            string format = "yyyyMMddHHmmss";
            int number;
            var output = new StringBuilder();

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');

                // サーバーアドレスごとの件数が直近m回分以上の場合、超えた分の一番古いデータを削除
                if (readPinglist.Where(x => x.ServerAddress == values[1])
                                        .Select(x => x)
                                        .Count() >= lastNumberOfTimes)
                {
                    var element = readPinglist.Where(x => x.ServerAddress == values[1]).FirstOrDefault();
                    readPinglist.Remove(element);
                }

                // タイムアウトの場合
                if (values[2] == "-")
                {
                    if (pingCountDic.ContainsKey(values[1]))
                    {
                        // 回数をカウントアップ
                        pingCountDic[values[1]]++;
                    }
                    else
                    {
                        // キーが存在しない場合は回数をセット
                        pingCountDic.Add(values[1], 1);
                        startTimeDic.Add(values[1], DateTime.ParseExact(values[0], format, null));
                    }

                    readPinglist.Add(new LogInfo()
                    {
                        LogTime = DateTime.ParseExact(values[0], format, null),
                        ServerAddress = values[1],
                        ReactionMiliSecond = null,
                        Subnet = GetSubnetAddress(values[1])
                    });
                }
                // 応答がある場合
                else if (int.TryParse(values[2], out number))
                {
                    if (pingCountDic.ContainsKey(values[1]))
                    {
                        if (pingCountDic[values[1]] >= timeOutCount)
                        {
                            endTimeDic.Add(values[1], DateTime.ParseExact(values[0], format, null));
                            brokenServerDic.Add(values[1], endTimeDic[values[1]] - startTimeDic[values[1]]);
                        }

                        // 回数のカウントを削除
                        pingCountDic.Remove(values[1]);
                        startTimeDic.Remove(values[1]);
                    }

                    readPinglist.Add(new LogInfo()
                    {
                        LogTime = DateTime.ParseExact(values[0], format, null),
                        ServerAddress = values[1],
                        ReactionMiliSecond = int.Parse(values[2]),
                        Subnet = GetSubnetAddress(values[1])
                    });
                }
            }

            foreach (var pair in brokenServerDic)
            {
                output.Append($"故障状態のサーバアドレス：{pair.Key}, 故障期間：{pair.Value.ToString()}\r\n");
            }

            var calcuratedServerAddressList = new List<string>();

            foreach (var item in readPinglist)
            {
                if (!calcuratedServerAddressList.Contains(item.ServerAddress))
                {
                    int calcuratedMiliSecond = readPinglist.Where(x => x.ServerAddress == item.ServerAddress)
                                                                            .Sum(x => x?.ReactionMiliSecond)
                                         / readPinglist.Where(x => x.ServerAddress == item.ServerAddress).Count() ?? 0;

                    if (calcuratedMiliSecond > reactionTime)
                    {
                        output.Append(item.ServerAddress
                                        + "(過負荷状態期間)："
                                        + calcuratedMiliSecond
                                        + "ミリ秒\r\n");
                        calcuratedServerAddressList.Add(item.ServerAddress);
                    }
                }
            }

            return output.ToString().TrimEnd('\r', '\n');
        }

        /// <summary>
        /// サブネットのネットワークアドレスを取得
        /// </summary>
        /// <param name="serverAddress">サーバアドレス</param>
        /// <returns>サブネットのネットワークアドレス</returns>
        private static string GetSubnetAddress(string serverAddress)
        {
            // 定数
            const int TOTAL_BITS = 32;
            const int NUMBER_OF_BYTES = 8;
            const int BINARY = 2;

            // 変数
            int subnet = int.Parse(serverAddress.Split('/')[1]);
            var binaryList = new List<string>();
            var elementSb = new StringBuilder();
            var result = new List<string>();

            // サーバアドレスを2進数に置き換える -> binaryList (置き換え先)
            foreach (string value in serverAddress.Split('/')[0].Split('.'))
            {
                binaryList.Add(Convert.ToString(int.Parse(value), BINARY).PadLeft(NUMBER_OF_BYTES, '0'));
            }

            // ホストアドレスを作成 -> binarySb (作成先)
            StringBuilder binarySb = new StringBuilder(string.Join("", binaryList));
            binarySb.Remove(subnet - 1, TOTAL_BITS - subnet);
            
            for (int i = subnet; i < string.Join("", binaryList).Length; i++)
            {
                binarySb.Append("0");
            }

            // バイト数毎に分割 -> binaryList (格納先)
            binaryList.Clear();
            
            for (int i = 0; i < binarySb.Length; i++)
            {
                elementSb.Append(binarySb[i]);
                if (elementSb.Length == NUMBER_OF_BYTES)
                {
                    binaryList.Add(elementSb.ToString());
                    elementSb.Clear();
                }
            }


            // ネットワークアドレスを2進数から10進数に変換 -> result
            foreach (string value in binaryList)
            {
                result.Add(Convert.ToInt32(value, 2).ToString());
            }

            // サブネットマスク付きでネットワークアドレスを返却
            return string.Join(".", result) + "/" + subnet.ToString();
        }
    }
}
