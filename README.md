## 🚀 Getting Started
Follow these steps to run the application on your local machine.

### 🛠️ Configuration
#### 🔐 OrderService - PostgreSQL Connection String
Open the OrderService/appsettings.json file and update the ConnectionStrings:DefaultConnection setting with your local PostgreSQL details:
```
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=OrderDb;Username=your_user;Password=your_password"
}
```

#### 🚀 Running Redis (required for BasketService)
#### ▶️ Running the Services
```
IdentityService

ProductService

BasketService

OrderService

ApiGateway
```

### 🧪 Test the endpoints
#### 🔐 Identity Service 

Regiset :
```
http://localhost:5000/api/identity/register
```

Body :
```
{
    "email": "test@gmail.com",
    "password": "Password_123",
    "confirmPassword": "Password_123"
}
```

Login :
```
http://localhost:5000/api/identity/login
```
Body:
```
{
    "email": "test@gmail.com",
    "password": "Password_123"
}
```


#### 📦 Product Service

Get Products:
```
http://localhost:5000/api/products/
```
Post Product :
```
http://localhost:5000/api/products/
```
Body :
```
{
  "name": "Board",
  "description": "wooden",
  "price": 2,
  "stock": 3
}
```
Filter Product :
```
http://localhost:5000/api/products/filter?description=300ml
```

#### 🛒 Basket Service

Add Item :
```
http://localhost:5000/api/basket/
```
Body :
```
{
  "userName": "test@gmail.com",
  "items": [
    {
      "quantity": 2,
      "price": 135.00,
      "productId": "2e5575eb-3a47-4525-bae2-6eac0d6b3bc2",
      "productName": "Cup"
    }
  ]
}
```
Remove item :
```
http://localhost:5000/api/basket/test@gmail.com/items/2e5575eb-3a47-4525-bae2-6eac0d6b3bc2
```
#### 🧾 Order Service

Get Orders:
```
http://localhost:5000/api/orders/
```

Add Item :
```
http://localhost:5000/api/orders/
```
Body:
```
{
  "userName": "test@gmail.com",
  "totalPrice": 121,
  "orderItems": [
    {
      "productId": "92B4410B-9749-45EB-9813-46B406F86974",
      "productName": "milk",
      "quantity": 1,
      "price": 2
    }
  ]
}
```
Filter :
```
http://localhost:5000/api/orders/filter?username=test@gmail.com
```





















