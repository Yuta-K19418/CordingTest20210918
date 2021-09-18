﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CordingTest20210918
{
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(@args[0]);
            var startTimeDic = new Dictionary<string, DateTime>();
            var endTimeDic = new Dictionary<string, DateTime>();
            var brokenServerDic = new Dictionary<string, TimeSpan>();
            var pingCountDic = new Dictionary<string, int>();
            int timeOutCount = Int32.Parse(args[1]);
            int reactionTime = Int32.Parse(args[2]);
            string format = "yyyyMMddHHmmss";
            int number;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] values = line.Split(',');
                
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
                }
                // 応答がある場合
                else if(int.TryParse(values[2], out number))
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
                }
            }

            foreach (var pair in brokenServerDic)
            {
                Console.WriteLine($"故障状態のサーバアドレス：{pair.Key}, 故障期間：{pair.Value.ToString()}");
            }
        }
    }
}