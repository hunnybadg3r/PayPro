![](https://i.imgur.com/zdK5UB1.png)

# PayPro
- 주유소 POS 프로그램과 연동하여 결제 프로세스 수행을 시뮬레이션하는 Demo App입니다.
- C#과 WPF를 활용하여 .NET 8을 기반으로 작성되었습니다.
## 주요 기능
- 주요 정보를 파악하기 쉽게 시각화하여 표시합니다.
- 결제 프로세스 시뮬레이션을 수행합니다.
## 프로젝트 구조
- MVVM + Services
	- 모듈간 의존성을 최소화하여 확장성과 유연성을 높였습니다. 

<img src="https://i.imgur.com/Bu2GQKn.png" width="600" />

### PayPro (Infrastructure Layer)
- 앱의 시작과 종료, 생명주기 관리
- Dependency Injection 구성, 관리
- Appsettings.json 
	- 로깅 설정
	- 통신 설정. 시리얼 포트, API의 BaseUrl 설정
### PayPro.Main (Presentation Layer)
- UI 표시 
	- MainWindow의 뷰와 뷰모델 정의
	- Custom Control 정의
### PayPro.Services (Application Layer)
- 결제 서비스와 POS 통신 서비스 정의
- 애플리케이션의 비즈니스 로직을 처리
### PayPro.Contracts (Domain Layer)
- 요청과 응답 데이터 계약 정의 (MockPos와 MockPayAPI)
- 모듈 간 메시지 교환을 위한 메시지 구조 정의 
	- 참조 없이 분리된 모듈 간(PayPro.Main와 PayPro.Services)의 메시지 전송을 지원
 
## 시뮬레이션 방식
![](https://i.imgur.com/SRePzIG.png)

- 가상의 POS(MockPOS)로 부터 결제 요청을 수신합니다. ①
- POS의 결제 요청을 가상의 Payment Gateway(MockPayAPI)로 전달하여 결제 프로세스를 수행합니다. ②③
- 결제 결과를 POS로 전달합니다. ④ 

<details>
  <summary>더보기(gif)</summary>
  
  ![demo-gif](https://i.imgur.com/78CbHq8.gif)

</details>

### MockPOS
- WPF(.NET 8)로 작성되었습니다.
- 시리얼 통신을 통해 PayPro에 결제 요청을 전송합니다.
- 실제 주유소 POS에서 쓰이는 패킷을 간소화하여 가상의 패킷을 정의하였습니다. 
	- [MockPOS.md](https://github.com/hunnybadg3r/PayPro/blob/master/MockPOS.md)

### MockPayAPI
- minimal API(.NET 8)로 작성되었습니다.
- PayPro로부터 결제 요청을 받으면 승인 혹은 실패로 응답합니다. 
	- [MockPayAPI.md](https://github.com/hunnybadg3r/PayPro/blob/master/MockPayAPI.md)

## 기술 
- .NET 8
- Dependency Injection
- Community Toolkit MVVM
- WPF Custom Control
### NuGet
- [LiveCharts](https://livecharts.dev/)
- [WPF UI](https://wpfui.lepo.co/)
- [Serilog](https://serilog.net/)
