using System;
using Xunit;
using CordingTest20210918;
using Xunit.Abstractions;

namespace UnitTest
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;
        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(DisplayName = "設問1")]
        public void Test1()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test1.txt", 1, 1, 1);
            Assert.Equal("故障状態のサーバアドレス：10.20.30.2/16, 故障期間：00:01:00\r\n"
                                + "故障状態のサーバアドレス：10.20.30.1/16, 故障期間：00:01:06", result);

        }

        [Fact(DisplayName = "設問2")]
        public void Test2()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test2.txt",2, 3, 1);
            Assert.Equal("故障状態のサーバアドレス：10.20.30.1/16, 故障期間：00:00:04\r\n"
                                + "故障状態のサーバアドレス：10.20.30.2/16, 故障期間：00:00:05", result);

        }
    }
}
