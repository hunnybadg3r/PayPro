# MockPayAPI 

MockPayAPI�� ������ ���� ����Ʈ���� ������ �����ϴ� API�Դϴ�. 
�� API�� .NET 8 ����� Minimal API�� �����Ǿ�����, ���� ��û�� ó���ϰ� ������ �����մϴ�.

## ��������Ʈ

### POST /api/payment

���� ��û�� ó���ϰ� ���� ����� ��ȯ�մϴ�.

#### ��û (Request)

```json
{
  "transactionId": "0624-000001",
  "amount": 50000,
  "paymentIdentifier": "12345678901234",
  "merchantId": "HDOBO1"
}
```

#### ���� (Response)
������ result �ʵ�� ���� enum �� �� �ϳ��� ��ȯ�մϴ�:
- 0: Success (����)
- 1: Declined (����)
- 2: Error (����)

##### ���� �� (200 OK):
```json
{
  "transactionId": "0624-000001",
  "result": "0", 
  "message": "Payment successful",
  "authorizationCode": "0001",
  "transactionDateTime": "2024-06-24T23:25:00.1234567Z"
}
```

##### ���� �� (422 Unprocessable Entity):
```json
{
  "transactionId": "0624-000001",
  "result": "1",
  "message": "Insufficient balance",
  "authorizationCode": "",
  "transactionDateTime": "2024-06-24T23:25:00.1234567Z"
}
```

##### �߸��� ��û �� (400 Bad Request):
```json
{
  "error": "Error message describing the issue"
}
```