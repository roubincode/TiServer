# TiServer
这是一个C#语言开发的服务器应用框架，主要应用于网络游戏的实时同步，因为作者一直致力于taikr.com这样一个游戏开发的交流学习网站的传播与发展，所以命名为TiServer。  
TiServer并非提供一个基础框架，而是基于ETServer提供的一些核心框架功能，开发了用于MMO，RTS等各类典型网络游戏的实战应用模块功能。
目的是可以提供给各种个人与公司直接利用TiServer开发完成的功能API，尽量少的或者根本不用自己开发服务端代码，在前端游戏中直接应用这些API即可实现各种微型网络游戏，或MMO,RTS多人网络游戏。  

## 使用指南
1 安装.NetCore SDK (ETCore4.0，5.0使用netcore2.2.300+,ETCore6.0使用netcore3.0+)   
2 通过vs installer安装 .net framework (共享组件需要安装在C盘)  
3 设置launch.json文件，在vs code中对项目进行调试  
4 如果安装了多个版本的.netcore用global.json指定项目net sdk版本  
### 前端运行  
1 客户端要求unity2017.4以上  
### 后端运行  
1 用Visaul Studio 打开Server解决方案  
2 编译Server/Hotfix/Server.Hotfix.csproj　(用命令行或用visaul studio单独编译都可以)  
3 包还原完成后，调试运行服务端  

### 应用框架已经或将在后续不断实现的功能接口
#### 分布式服务器
地图服务器的物理主机是可以不断添加的  
#### 可选区服策略
固定区服：需要选服创建账号
动态区服：没有固定区服，可以分线，换线  
#### 独立的用户认证体系
不管多少具体的游戏，都是同一个用户认证系统
角色昵称在同一个游戏中不可重复  
#### 通用网络游戏同步接口  
登录,注册账号  
同步单元:房间的创建与管理  
角色移动状态同步  
角色技能攻击同步  
任务系统管理与任务同步  
不断增加中  
#### 可选网络游戏同步接口  
不断增加中  

### TiServer网络教学MMORPG课程案例教学
https://www.taikr.com/course/1076

## 交流与讨论：  
[肉饼学习交流网站：](http://www.taikr.com) http://www.taikr.com  
__讨论QQ群 : 695494071__  
__作者邮箱 : liaoxiangning@taikr.com

