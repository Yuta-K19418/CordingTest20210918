# 動作環境
CordingTest20210918.exeは[.NETCoreSDK](https://dotnet.microsoft.com/download/dotnet/5.0)のインストール後、  
Windows、Linux、MacOSで使用可能です。  
GitHubからclone後CordingTest20210918配下で
```
dotnet restore
dotnet build
```
を実行します。

# 使用方法

```
>CordingTest20210918.exe
監視ログファイルのパスを入力してください。
D:\C#\C#project\source\repos\CordingTest20210918\UnitTest\test4.txt
N回以上連続してタイムアウトした場合のタイムアウト回数(N)を入力してください。
2
直近m回の平均応答時間がtミリ秒を超えた場合の直近回数(m)を入力してください。
5
直近m回の平均応答時間がtミリ秒を超えた場合のミリ秒(t)を入力してください。
10
故障状態のサーバアドレス：10.20.30.1/16, 故障期間：00:03:06
故障状態のサーバアドレス：10.20.30.2/16, 故障期間：00:05:00
故障状態のサーバアドレス：192.168.1.1/24, 故障期間：01:03:06
全サーバが故障しているサブネット：10.20.0.0/16, ネットワークの故障期間：00:03:06
```