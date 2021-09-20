# 動作環境
CordingTest20210918.exeは[.NETCoreSDK](https://dotnet.microsoft.com/download/dotnet/5.0)のインストール後、  
Windows、Linux、MacOSで使用可能です。  
GitHubからclone後CordingTest20210918配下で
```
$ cd CordingTest20210918
$ dotnet restore
  復元対象のプロジェクトを決定しています...
  ${Cloneディレクトリ}/CordingTest20210918/CordingTest20210918/CordingTest20210918.csproj を復元しました (91 ms)。
  ${Cloneディレクトリ}/CordingTest20210918/UnitTest/UnitTest.csproj を復元しました (623 ms)。
$ dotnet build --configuration Release
.NET 向け Microsoft (R) Build Engine バージョン 16.11.0+0538acc04
Copyright (C) Microsoft Corporation.All rights reserved.

  復元対象のプロジェクトを決定しています...
  復元対象のすべてのプロジェクトは最新です。
  CordingTest20210918 -> ${Cloneディレクトリ}/CordingTest20210918/CordingTest20210918/bin/Release/net5.0/CordingTest20210918.dll
  UnitTest -> ${Cloneディレクトリ}/CordingTest20210918/UnitTest/bin/Release/net5.0/UnitTest.dll

ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:02.65
```
上記のように``dotnet restore``、``dotnet build --configuration Release``を実行します。

# 使用方法
``${Cloneディレクトリ}/CordingTest20210918/CordingTest20210918/bin/Release/net5.0``配下に  
``CordingTest20210918``というファイルが作成されているので、実行します。

```
$ ./CordingTest20210918
監視ログファイルのパスを入力してください。
/app/CordingTest20210918/UnitTest/test4.txt
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

# プログラム構成
以下のような構成となっています。
[!](./img/CordingTest20210918.png)