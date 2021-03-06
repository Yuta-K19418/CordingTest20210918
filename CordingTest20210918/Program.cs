using System;
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
            Console.WriteLine("監視ログファイルのパスを入力してください。");
            string inputFilePath = @Console.ReadLine();

            // パラメータが有効かどうかの確認
            if (!InputFilePathIsValid(inputFilePath))
            {
                Console.WriteLine("ファイルパスが不正です。");
                return;
            }

            Console.WriteLine("N回以上連続してタイムアウトした場合のタイムアウト回数(N)を入力してください。");
            string timeOutCountString = Console.ReadLine();
            int timeOutCount = 0;

            // パラメータが有効かどうかの確認
            if (!InputIsValid(timeOutCountString))
            {
                Console.WriteLine("パラメータが不正です。");
                return;
            }
            else
            {
                timeOutCount = Int32.Parse(timeOutCountString);
            }

            Console.WriteLine("直近m回の平均応答時間がtミリ秒を超えた場合の直近回数(m)を入力してください。");
            string lastNumberOfTimesString = Console.ReadLine();
            int lastNumberOfTimes = 0;

            // パラメータが有効かどうかの確認
            if (!InputIsValid(lastNumberOfTimesString))
            {
                Console.WriteLine("パラメータが不正です。");
                return;
            }
            else
            {
                lastNumberOfTimes = Int32.Parse(lastNumberOfTimesString);
            }

            Console.WriteLine("直近m回の平均応答時間がtミリ秒を超えた場合のミリ秒(t)を入力してください。");
            string reactionTimeString = Console.ReadLine();
            int reactionTime = 0;

            // パラメータが有効かどうかの確認
            if (!InputIsValid(reactionTimeString))
            {
                Console.WriteLine("パラメータが不正です。");
                return;
            }
            else
            {
                reactionTime = Int32.Parse(reactionTimeString);
            }

            string output = OutputPeriodOfBrokenServer(inputFilePath,
                                                        timeOutCount,
                                                        lastNumberOfTimes,
                                                        reactionTime);
            Console.WriteLine(output);
        }

        /// <summary>
        /// 監視ログファイルが存在するか
        /// </summary>
        /// <param name="inputFilePath">監視ログファイルパス</param>
        /// <returns>Yes/No</returns>
        public static bool InputFilePathIsValid(string inputFilePath)
        {
            return File.Exists(inputFilePath) ? true : false;
        }

        /// <summary>
        /// 入力文字列が有効か
        /// </summary>
        /// <param name="input">入力文字列</param>
        /// <returns>Yes/No</returns>
        public static bool InputIsValid(string input)
        {
            int number;
            return int.TryParse(input, out number) && number >= 0 ? true : false;
        }

        /// <summary>
        /// 故障状態のサーバアドレスとそのサーバの故障期間を取得
        /// </summary>
        /// <param name="inputFilePath">監視ログファイルパス</param>
        /// <param name="timeOutCount">タイムアウト回数(N)</param>
        /// <param name="lastNumberOfTimes">直近の回数(M)</param>
        /// <param name="reactionTime">応答時間(tミリ秒)</param>
        /// <returns>出力文字列</returns>
        public static string OutputPeriodOfBrokenServer(string inputFilePath,
                                                        int timeOutCount,
                                                        int lastNumberOfTimes,
                                                        int reactionTime)
        {
            // 変数
            var sr = new StreamReader(@inputFilePath);
            var logInfoList = new List<LogInfo>();
            var serverInfoList = new List<ServerInfo>();
            var calcuratedServerAddressList = new List<string>();
            var subnetInfoList = new List<SubnetInfo>();
            var output = new StringBuilder();

            // ログファイルから読み込み -> logInfoList
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                string format = "yyyyMMddHHmmss";
                int number;

                // サーバーアドレスごとの件数が直近m回分以上の場合、超えた分の一番古いデータを対象外にする
                if (logInfoList.Where(x => x.ServerAddress == values[1])
                                        .Select(x => x)
                                        .Count() >= lastNumberOfTimes)
                {
                    logInfoList.Where(x => x.ServerAddress == values[1])
                                .Select(x => x)
                                .First(x => x.ShouldExclude = true);
                }

                logInfoList.Add(new LogInfo()
                {
                    LogTime = DateTime.ParseExact(values[0], format, null),
                    ServerAddress = values[1],
                    ReactionMiliSecond = int.TryParse(values[2], out number) ? int.Parse(values[2]) : null,
                    SubnetAddress = GetSubnetAddress(values[1]),
                    ShouldExclude = false
                });
            }

            // logInfoListから故障状態のサーバアドレスとそのサーバの故障期間を集計 -> serverInfoList (集計結果)
            foreach (var logInfo in logInfoList)
            {
                // 応答がある場合
                if (logInfo.ReactionMiliSecond != null)
                {
                    // serverInfoListにServerAddress、SubnetAddressが一致する、集計中の場合
                    if (serverInfoList.Where(x =>
                                        x.ServerAddress == logInfo.ServerAddress
                                        && x.IsAggregating
                                        && x.SubnetAddress == logInfo.SubnetAddress)
                                        .Any())
                    {
                        // 集計完了にする
                        serverInfoList.Where(x =>
                                        x.ServerAddress == logInfo.ServerAddress
                                        && x.IsAggregating
                                        && x.SubnetAddress == logInfo.SubnetAddress)
                                        .Select(x =>
                                        {
                                            x.HasBroken = (x.PingCount >= timeOutCount) ? true : false;
                                            x.IsAggregating = false;
                                            x.EndTime = logInfo.LogTime;
                                            x.ReactionTime = logInfo.LogTime - x.StartTime;
                                            return x;
                                        })
                                        .ToList();
                    }
                    // 該当しない場合
                    else
                    {
                        serverInfoList.Add(new ServerInfo()
                        {
                            ServerAddress = logInfo.ServerAddress,
                            HasBroken = false,
                            IsAggregating = false,
                            StartTime = logInfo.LogTime,
                            EndTime = logInfo.LogTime,
                            ReactionTime = TimeSpan.FromMilliseconds(logInfo.ReactionMiliSecond ?? 0),
                            SubnetAddress = logInfo.SubnetAddress
                        });
                    }
                }
                // タイムアウトの場合
                else
                {
                    // serverInfoListにServerAddress、SubnetAddressが一致する、集計中の場合
                    if (serverInfoList.Where(x =>
                                        x.ServerAddress == logInfo.ServerAddress
                                        && x.IsAggregating
                                        && x.SubnetAddress == logInfo.SubnetAddress)
                                        .Any())
                    {
                        serverInfoList.Where(x =>
                                        x.ServerAddress == logInfo.ServerAddress
                                        && x.IsAggregating
                                        && x.SubnetAddress == logInfo.SubnetAddress)
                                        .Select(x => x.PingCount = (x.PingCount ?? 0) + 1)
                                        .ToList();
                    }
                    // 該当しない場合
                    else
                    {
                        serverInfoList.Add(new ServerInfo()
                        {
                            ServerAddress = logInfo.ServerAddress,
                            IsAggregating = true,
                            StartTime = logInfo.LogTime,
                            PingCount = 1,
                            SubnetAddress = logInfo.SubnetAddress
                        });
                    }
                }
            }

            // 故障サーバ、故障期間の出力
            foreach (var serverInfo in serverInfoList.Where(x => x.HasBroken))
            {
                output.Append($"故障状態のサーバアドレス：{serverInfo.ServerAddress},"
                    + $" 故障期間：{serverInfo.ReactionTime}\r\n");
            }

            // 各サーバにおいて、過負荷状態となっている期間を出力
            // (直近m回なので、logInfoListにおいてShouldExcludeがtrueのものを対象外とする)
            foreach (var logInfo in logInfoList.Where(x => !x.ShouldExclude))
            {
                // 過負荷状態としてまだ出力されていない場合
                if (!calcuratedServerAddressList.Contains(logInfo.ServerAddress))
                {
                    // 直近m回の平均応答時間 -> item.ServerAddressの合計応答時間 ÷ logInfoListのServerAddressの件数
                    int calcuratedMiliSecond = logInfoList.Where(x => x.ServerAddress == logInfo.ServerAddress
                                                                        && !x.ShouldExclude)
                                                                            .Sum(x => x.ReactionMiliSecond ?? 0)
                                                 / logInfoList.Where(x => x.ServerAddress == logInfo.ServerAddress
                                                                        && !x.ShouldExclude)
                                                                .Count();

                    // 直近m回の平均応答時間がtミリ秒を超えた場合
                    if (calcuratedMiliSecond > reactionTime)
                    {
                        output.Append(logInfo.ServerAddress
                                        + "(過負荷状態期間)："
                                        + calcuratedMiliSecond
                                        + "ミリ秒\r\n");
                        // 出力したServerAddressはcalcuratedServerAddressListへ追加
                        calcuratedServerAddressList.Add(logInfo.ServerAddress);
                    }
                }
            }

            // サーバが全て故障しているサブネットアドレスを抽出 -> subnetInfoList
            foreach (var serverInfo in serverInfoList)
            {
                // subnetInfoListにserverInfoのサブネットアドレスが存在しない、かつサーバが故障状態の場合
                if (!subnetInfoList.Where(x => x.SubnetAddress == serverInfo.SubnetAddress)
                                     .Any() 
                                && serverInfo.HasBroken)
                {
                    subnetInfoList.Add(new SubnetInfo() 
                                                {
                                                    ServerAddressList = new List<string>()
                                                    {
                                                        serverInfo.ServerAddress
                                                    },
                                                    SubnetAddress = serverInfo.SubnetAddress,
                                                    StartTime = serverInfo.StartTime,
                                                    EndTime = serverInfo.EndTime,
                                                    HasBroken = serverInfo.HasBroken,
                                                    
                                                });
                }
                // subnetInfoListにデータがする、かつサーバが故障状態の場合
                else if (subnetInfoList.Any() && serverInfo.HasBroken)
                {
                    //  StartTimeよりserverInfo.StartTimeの方が早い場合
                    if (subnetInfoList.Where(x => x.SubnetAddress == serverInfo.SubnetAddress)
                                        .Select(x => x.StartTime)
                                        .First()
                                > serverInfo.StartTime)
                    {
                        // StartTimeを更新
                        subnetInfoList.Where(x => x.SubnetAddress == serverInfo.SubnetAddress)
                                                .Select(x => x.StartTime = serverInfo.StartTime)
                                                .ToList();
                    }

                    // EndTimeよりserverInfo.EndTimeの方が早い場合
                    if (subnetInfoList.Where(x => x.SubnetAddress == serverInfo.SubnetAddress)
                                        .Select(x => x.EndTime)
                                        .First()
                                > serverInfo.EndTime)
                    {
                        // EndTimeを更新
                        subnetInfoList.Where(x => x.SubnetAddress == serverInfo.SubnetAddress)
                                                .Select(x => x.EndTime = serverInfo.EndTime)
                                                .ToList();
                    }

                    // ServerAddressListにserverInfo.ServerAddressが存在しない場合
                    if (!subnetInfoList.Where(x => x.SubnetAddress == serverInfo.SubnetAddress)
                                        .Select(x => x.ServerAddressList)
                                        .First()
                                        .Contains(serverInfo.ServerAddress))
                    {
                        // ServerAddressListにServerAddressを追加
                        subnetInfoList.Where(x => x.SubnetAddress == serverInfo.SubnetAddress)
                                        .Select(x => x.ServerAddressList)
                                        .First()
                                        .Add(serverInfo.ServerAddress);
                    }
                }

                // 故障していないサーバが存在する場合
                if (!serverInfo.HasBroken)
                {
                    // subnetInfoListから該当するSubnetAddressを削除
                    subnetInfoList.RemoveAll(x => x.SubnetAddress == serverInfo.SubnetAddress);
                }
            }

            // あるサブネット内のサーバが全て故障 = サブネットにサーバが2台以上存在する場合 -> サブネットの削除
            subnetInfoList.RemoveAll(x => x.ServerAddressList.Count < 2);

            // 各サブネット毎にネットワークの故障期間を出力
            foreach (var serverInfo in subnetInfoList)
            {
                output.Append($"全サーバが故障しているサブネット：{serverInfo.SubnetAddress},"
                    + $" ネットワークの故障期間：{serverInfo.EndTime - serverInfo.StartTime}\r\n");
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
            foreach (string part in serverAddress.Split('/')[0].Split('.'))
            {
                binaryList.Add(Convert.ToString(int.Parse(part), BINARY).PadLeft(NUMBER_OF_BYTES, '0'));
            }

            // ホストアドレスを作成 -> binarySb (作成先)
            var binarySb = new StringBuilder(string.Join("", binaryList));
            binarySb.Remove(subnet, TOTAL_BITS - subnet);
            
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
            foreach (string binaryElement in binaryList)
            {
                result.Add(Convert.ToInt32(binaryElement, 2).ToString());
            }

            // サブネットマスク付きでネットワークアドレスを返却
            return string.Join(".", result) + "/" + subnet.ToString();
        }
    }
}
