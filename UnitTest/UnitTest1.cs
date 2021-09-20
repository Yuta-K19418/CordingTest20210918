using System;
using Xunit;
using CordingTest20210918;

namespace UnitTest
{
    public class UnitTest1
    {

        [Fact(DisplayName = "�t�@�C���p�X���L��")]
        public void FilePathIsValid()
        {
            bool result = Program.InputFilePathIsValid(@"D:\C#\C#project\source\repos\CordingTest20210918\UnitTest\test1.txt");
            Assert.True(result);
        }

        [Fact(DisplayName = "�t�@�C���p�X������")]
        public void FilePathIsInvalid()
        {
            bool result = Program.InputFilePathIsValid(@"D:\C#\aaaa.txt");
            Assert.False(result);
        }

        [Fact(DisplayName = "���̐����̃p�����[�^")]
        public void InputParamterIsValid()
        {
            bool result = Program.InputIsValid("1");
            Assert.True(result);
        }

        [Fact(DisplayName = "���̐����̃p�����[�^")]
        public void InputParamterIsInvalid1()
        {
            bool result = Program.InputIsValid("-1");
            Assert.False(result);
        }

        [Fact(DisplayName = "�����ȊO�̃p�����[�^")]
        public void InputParamterIsInvalid2()
        {
            bool result = Program.InputIsValid("��");
            Assert.False(result);
        }

        [Fact(DisplayName = "�ݖ�1")]
        public void Test1()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test1.txt", 1, 10, 500);
            Assert.Equal("�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.2/16, �̏���ԁF00:01:00\r\n"
                                + "�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.1/16, �̏���ԁF00:01:06", result);

        }

        [Fact(DisplayName = "�ݖ�2")]
        public void Test2()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test2.txt",2, 3, 5);
            Assert.Equal("�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.1/16, �̏���ԁF00:00:04\r\n"
                                + "�̏��Ԃ̃T�[�o�A�h���X�F192.168.1.1/24, �̏���ԁF00:00:05", result);

        }

        [Fact(DisplayName = "�ݖ�3")]
        public void Test3()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test3.txt", 2, 3, 10);
            Assert.Equal("�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.2/16, �̏���ԁF00:05:00\r\n"
                                + "10.20.30.1/16(�ߕ��׏�Ԋ���)�F131�~���b\r\n"
                                + "192.168.1.1/24(�ߕ��׏�Ԋ���)�F11�~���b", result);

        }

        [Fact(DisplayName = "�ݖ�4")]
        public void Test4()
        {
            string result = Program.OutputPeriodOfBrokenServer("./test4.txt", 2, 5, 10);
            Assert.Equal("�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.1/16, �̏���ԁF00:03:06\r\n"
                                + "�̏��Ԃ̃T�[�o�A�h���X�F10.20.30.2/16, �̏���ԁF00:05:00\r\n"
                                + "�̏��Ԃ̃T�[�o�A�h���X�F192.168.1.1/24, �̏���ԁF01:03:06\r\n"
                                + "�S�T�[�o���̏Ⴕ�Ă���T�u�l�b�g�F10.20.0.0/16, �l�b�g���[�N�̌̏���ԁF00:03:06", result);

        }
    }
}
