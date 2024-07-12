# MockPayAPI 

MockPayAPI는 가상의 결제 게이트웨이 역할을 수행하는 API입니다. 
이 API는 .NET 8 기반의 Minimal API로 구현되었으며, 결제 요청을 처리하고 응답을 생성합니다.

## 엔드포인트

### POST /api/payment

결제 요청을 처리하고 결제 결과를 반환합니다.

#### 요청 (Request)

```json
{
  "transactionId": "0624-000001",
  "amount": 50000,
  "paymentIdentifier": "12345678901234",
  "merchantId": "HDOBO1"
}
```

#### 응답 (Response)
응답의 result 필드는 다음 enum 값 중 하나를 반환합니다:
- 0: Success (성공)
- 1: Declined (거절)
- 2: Error (오류)

##### 성공 시 (200 OK):
```json
{
  "transactionId": "0624-000001",
  "result": "0", 
  "message": "Payment successful",
  "authorizationCode": "0001",
  "transactionDateTime": "2024-06-24T23:25:00.1234567Z"
}
```

##### 실패 시 (422 Unprocessable Entity):
```json
{
  "transactionId": "0624-000001",
  "result": "1",
  "message": "Insufficient balance",
  "authorizationCode": "",
  "transactionDateTime": "2024-06-24T23:25:00.1234567Z"
}
```

##### 잘못된 요청 시 (400 Bad Request):
```json
{
  "error": "Error message describing the issue"
}
```