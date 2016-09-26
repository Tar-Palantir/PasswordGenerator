# 密码生成器

尚在开发初期，计划实现Windows版，Android版和iOS版，主要用于个人。
```
输入keyword、长度、简化的代码（大写、小写字母，数字，符号的特定选择）生成，确定长度，且包含满足要求的密码。
目前支持大写、小写字母，数字，符号。可选择是否包含及必须包含的选择，符号可以对支持的符号单独选择。
```
已完成1.0版，基本功能已完成，但细节处理不足，异常情况处理较少，界面过于简陋。

## 下载

Release 1.0
* [Windows 绿色版](https://github.com/Tar-Palantir/PasswordGenerator/raw/master/Download/win/PasswordGenerator_1.0.16270.8.zip)
* [Android 安装包](https://github.com/Tar-Palantir/PasswordGenerator/raw/master/Download/android/PasswordGenerator.Android-1.02.apk)

## 结构说明


### 1) PasswordGenerator.Core
核心程序，采用PCL结构，加密库使用了PCLCrypto。

目标框架：
1. .NET Framework 4.5
2. ASP.NET Core 5.0
3. Windows 8
4. Windows Phone 8.1
5. Windows Phone Silverlight 8
6. Xamarin.Android
7. Xamarin.iOS
8. Xamarin.iOS(Classic)
### 2) PasswordGenerator.Console
控制台测试程序

.net框架版本：4.6.1
### 3) PasswordGenerator.WPF
Windows程序

.net框架版本 4.5
### 4) PasswordGenerator.Android
Android程序

#### Api版本信息
* 编译版本level19（Android 4.4）  
* 最小版本level15（Android 4.0.3）  
* 目标版本level19（Android 4.4）

#### 框架支持
* ARM
* ARM-v7a
* ARM64-v8a
* x86
* x86_64




