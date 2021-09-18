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

        [Fact(DisplayName = "�ݖ�1")]
        public void Test1()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test1.txt", 1, 1, 500);
            Assert.Equal("�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.2/16, �̏���ԁF00:01:00\r\n"
                                + "�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.1/16, �̏���ԁF00:01:06", result);

        }

        [Fact(DisplayName = "�ݖ�2")]
        public void Test2()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test2.txt",2, 3, 5);
            Assert.Equal("�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.1/16, �̏���ԁF00:00:04\r\n"
                                + "�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.2/16, �̏���ԁF00:00:05", result);

        }

        [Fact(DisplayName = "�ݖ�3")]
        public void Test3()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test3.txt", 2, 3, 10);
            Assert.Equal("�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.2/16, �̏���ԁF00:05:00\r\n"
                                + "192.168.1.1/24(�ߕ��׏�Ԋ���)�F11�~���b\r\n"
                                + "10.20.30.1/16(�ߕ��׏�Ԋ���)�F174�~���b", result);

        }
    }
}
