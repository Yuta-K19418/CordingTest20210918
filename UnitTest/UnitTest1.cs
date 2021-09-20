using System;
using Xunit;
using CordingTest20210918;

namespace UnitTest
{
    public class UnitTest1
    {

        [Fact(DisplayName = "ファイルパスが有効")]
        public void FilePathIsValid()
        {
            bool result = Program.InputFilePathIsValid(@"D:\C#\C#project\source\repos\CordingTest20210918\UnitTest\test1.txt");
            Assert.True(result);
        }

        [Fact(DisplayName = "ファイルパスが無効")]
        public void FilePathIsInvalid()
        {
            bool result = Program.InputFilePathIsValid(@"D:\C#\aaaa.txt");
            Assert.False(result);
        }

        [Fact(DisplayName = "正の数字のパラメータ")]
        public void InputParamterIsValid()
        {
            bool result = Program.InputIsValid("1");
            Assert.True(result);
        }

        [Fact(DisplayName = "負の数字のパラメータ")]
        public void InputParamterIsInvalid1()
        {
            bool result = Program.InputIsValid("-1");
            Assert.False(result);
        }

        [Fact(DisplayName = "数字以外のパラメータ")]
        public void InputParamterIsInvalid2()
        {
            bool result = Program.InputIsValid("あ");
            Assert.False(result);
        }

        [Fact(DisplayName = "設問1")]
        public void Test1()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test1.txt", 1, 10, 500);
            Assert.Equal("故障状態のサーバアドレス：10.20.30.2/16, 故障期間：00:01:00\r\n"
                                + "故障状態のサーバアドレス：10.20.30.1/16, 故障期間：00:01:06", result);

        }

        [Fact(DisplayName = "設問2")]
        public void Test2()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test2.txt",2, 3, 5);
            Assert.Equal("故障状態のサーバアドレス：10.20.30.1/16, 故障期間：00:00:04\r\n"
                                + "故障状態のサーバアドレス：192.168.1.1/24, 故障期間：00:00:05", result);

        }

        [Fact(DisplayName = "設問3")]
        public void Test3()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test3.txt", 2, 3, 10);
            Assert.Equal("故障状態のサーバアドレス：10.20.30.2/16, 故障期間：00:05:00\r\n"
                                + "10.20.30.1/16(過負荷状態期間)：131ミリ秒\r\n"
                                + "192.168.1.1/24(過負荷状態期間)：11ミリ秒", result);

        }

        [Fact(DisplayName = "設問4")]
        public void Test4()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test4.txt", 2, 5, 10);
            Assert.Equal("故障状態のサーバアドレス：10.20.30.1/16, 故障期間：00:03:06\r\n"
                                + "故障状態のサーバアドレス：10.20.30.2/16, 故障期間：00:05:00\r\n"
                                + "故障状態のサーバアドレス：192.168.1.1/24, 故障期間：01:03:06\r\n"
                                + "全サーバが故障しているサブネット：10.20.0.0/16, ネットワークの故障期間：00:03:06", result);

        }
    }
}
