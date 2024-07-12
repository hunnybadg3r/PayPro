![](https://i.imgur.com/zdK5UB1.png)

# PayPro
- 주유소 POS 프로그램과 연동하여 결제 프로세스 수행을 시뮬레이션하는 DEMO APP입니다.
- C#과 WPF를 활용하여 .NET 8을 기반으로 작성되었습니다.
## 주요기능
- 주요 정보를 파악하기 쉽게 시각화하여 표시합니다.
- 결제 프로세스 시뮬레이션을 수행합니다.
## 프로젝트 구조
- MVVM + Services
	- 모듈간 의존성을 최소화하여 확장성과 유연성을 높였습니다. 

![](https://i.imgur.com/pAKHmgy.png)

## 시뮬레이션 방식
1. 가상의 POS(MockPOS)로 부터 결제 요청을 수신합니다.
2. POS의 결제 요청을 가상의 Payment Gateway(MockPayAPI)로 전달하여 결제 프로세스를 수행합니다.

![](https://i.imgur.com/WQSV61c.gif)

### MockPOS

![](https://i.imgur.com/kfUHCIE.png)

- WPF(.NET 8)로 작성되었습니다.
- 시리얼 통신을 통해 PayPro에 결제 요청을 전송합니다.
- 실제 주유소 POS에서 쓰이는 패킷을 간소화하여 가상의 패킷을 정의하였습니다. 
	- [MockPOS.md](https://github.com/hunnybadg3r/PayPro/blob/master/MockPOS.md)
### MockPayAPI
- minimal API(.NET 8)로 작성되었습니다.
- PayPro로부터 결제 요청을 받으면 승인 혹은 실패로 응답합니다. 
	- [MockPayAPI.md](https://github.com/hunnybadg3r/PayPro/blob/master/MockPayAPI.md)

## 사용된 기술 
- Dependency Injection
- Community Toolkit MVVM
- WPF Custom Control
### NuGet
- [LiveChart2](https://livecharts.dev/)
- [WPF UI](https://wpfui.lepo.co/)
- [Serilog](https://serilog.net/)
