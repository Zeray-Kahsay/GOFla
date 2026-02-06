📚 Tech Stack
...............
# Backend
.NET 10
Entity Framework Core
SQL Server
Identity Core
JWT Authentication
FluentValidation
Stripe.NET
CloudinaryDotNet

# Frontend
React 19
TypeScript
Redux Toolkit
RTK Query
React Router
Tailwind CSS
React Hook Form
Zod
Lucide Icons


API ENDPOINTS 
---------------

# Authentication
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh-token
POST   /api/auth/revoke-token
POST   /api/auth/external-login
GET    /api/auth/me
POST   /api/auth/change-password

# Restaurants
GET    /api/restaurants
GET    /api/restaurants/{id}
POST   /api/restaurants (auth)
PUT    /api/restaurants/{id} (auth)
DELETE /api/restaurants/{id} (auth)

# Menu Items
/api/restaurants/{restaurantId}/menu-items      → Customer & Owner (GET)
/api/menu-items/{id}                             → Customer (GET)

/api/owner/restaurants/{restaurantId}/menu-items → Owner (POST, GET ALL)
/api/owner/menu-items/{id}                       → Owner (PUT, DELETE)
/api/owner/menu-items/{id}/toggle                → Owner (PATCH)
/api/owner/menu-items/{id}/image                 → Owner (POST)


# Cart
GET    /api/cart (auth)
POST   /api/cart/items (auth)
PUT    /api/cart/items/{id} (auth)
DELETE /api/cart/items/{id} (auth)
DELETE /api/cart/clear (auth)

# Orders
GET    /api/orders (auth)
GET    /api/orders/{id} (auth)
POST   /api/orders (auth)
POST   /api/orders/{id}/cancel (auth)

# Reviews 
GET    /api/orders (auth)
GET    /api/orders/{id} (auth)
POST   /api/orders (auth)
POST   /api/orders/{id}/cancel (auth)

# Favorites
GET    /api/favorites (auth)
GET    /api/favorites/check/{restaurantId} (auth)
POST   /api/favorites/{restaurantId} (auth)
DELETE /api/favorites/{restaurantId} (auth)

# Search
GET    /api/search
GET    /api/search/restaurants
GET    /api/search/menu-items
GET    /api/search/popular
GET    /api/search/suggestions

🚀 Database Setup
bash# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Create migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

📚 API Endpoints
Authentication

POST /api/auth/register - Register new user
POST /api/auth/login - Login
POST /api/auth/refresh-token - Refresh access token
POST /api/auth/revoke-token - Logout
POST /api/auth/external-login - OAuth2 login
GET /api/auth/me - Get current user (auth required)
POST /api/auth/change-password - Change password (auth required)

Restaurants

GET /api/restaurants - Get all restaurants (paginated)
GET /api/restaurants/{id} - Get restaurant by ID
POST /api/restaurants - Create restaurant (auth required)
PUT /api/restaurants/{id} - Update restaurant (auth required)
DELETE /api/restaurants/{id} - Delete restaurant (auth required)
PATCH /api/restaurants/{id}/toggle-active - Toggle active status (auth required)

Menu Items

GET /api/menuitems/{id} - Get menu item by ID
GET /api/menuitems/restaurant/{restaurantId} - Get all items by restaurant
GET /api/menuitems/restaurant/{restaurantId}/category/{category} - Get by category
POST /api/menuitems - Create menu item (auth required)
PUT /api/menuitems/{id} - Update menu item (auth required)
DELETE /api/menuitems/{id} - Delete menu item (auth required)
PATCH /api/menuitems/{id}/toggle-availability - Toggle availability (auth required)

Cart

GET /api/cart - Get user cart (auth required)
POST /api/cart/items - Add item to cart (auth required)
PUT /api/cart/items/{cartItemId} - Update quantity (auth required)
DELETE /api/cart/items/{cartItemId} - Remove item (auth required)
DELETE /api/cart/clear - Clear cart (auth required)

Orders

GET /api/orders - Get user orders (paginated, auth required)
GET /api/orders/{id} - Get order by ID (auth required)
GET /api/orders/number/{orderNumber} - Get by order number (auth required)
POST /api/orders - Create order (auth required)
POST /api/orders/{id}/cancel - Cancel order (auth required)
PATCH /api/orders/{id}/status - Update order status (auth required)

Addresses

GET /api/addresses - Get user addresses (auth required)
GET /api/addresses/{id} - Get address by ID (auth required)
POST /api/addresses - Create address (auth required)
PUT /api/addresses/{id} - Update address (auth required)
DELETE /api/addresses/{id} - Delete address (auth required)
PATCH /api/addresses/{id}/set-default - Set as default (auth required)

Images

POST /api/images/upload - Upload image (auth required)
POST /api/images/upload-with-size - Upload with transformation (auth required)
POST /api/images/restaurant/{restaurantId} - Upload restaurant image (auth required)
POST /api/images/menu-item/{menuItemId} - Upload menu item image (auth required)
DELETE /api/images?publicId={publicId} - Delete image (auth required)

Payment

GET /api/payment/config - Get Stripe publishable key
POST /api/payment/create-payment-intent - Create payment intent (auth required)
GET /api/payment/payment-methods - Get saved payment methods (auth required)
POST /api/payment/attach-payment-method - Attach payment method (auth required)
DELETE /api/payment/payment-methods/{id} - Remove payment method (auth required)
POST /api/payment/webhook - Stripe webhook (public)


🔑 Example API Requests
Register User
jsonPOST /api/auth/register
{
  "email": "user@example.com",
  "password": "Password123",
  "firstName": "John",
  "lastName": "Doe"
}
Login
jsonPOST /api/auth/login
{
  "email": "user@example.com",
  "password": "Password123"
}
Add to Cart
jsonPOST /api/cart/items
Authorization: Bearer {token}
{
  "menuItemId": 1,
  "quantity": 2,
  "specialInstructions": "No onions please"
}
Create Order
jsonPOST /api/orders
Authorization: Bearer {token}
{
  "deliveryAddressId": 1,
  "paymentMethodId": "pm_card_visa"
}

🔒 OAuth2 Setup Instructions
Google

Go to Google Cloud Console
Create a project
Enable Google+ API
Create OAuth 2.0 credentials
Add authorized redirect URI: https://yourdomain.com/signin-google

Facebook

Go to Facebook Developers
Create an app
Add Facebook Login product
Configure Valid OAuth Redirect URIs

GitHub

Go to Settings → Developer settings → OAuth Apps
Create a new OAuth App
Set callback URL: https://yourdomain.com/signin-github


🧪 Testing with Swagger

Run the application
Navigate to https://localhost:5001/swagger
Register a new user
Copy the JWT token from the response
Click "Authorize" button at the top
Enter: Bearer {your-token}
Now you can test protected endpoints


📝 Next Steps
Immediate

Set up external OAuth providers
Configure Stripe webhook endpoint
Set up Cloudinary account
Add database seed data


#### Client-side
## Cart to Payment architecture-flow
Cart → Order → Payment Intent → Payment Method → Webhook → Status → Realtime update
1️⃣ Review cart
2️⃣ Choose address
3️⃣ Create Order (server calculates totals)
4️⃣ Receive payment client secret
5️⃣ Show payment options (Card, Vipps, future providers)
6️⃣ Confirm payment
7️⃣ Order becomes Paid via webhook
## Step-1 = Review + Address
## Step-2 = Payment Method and Pay


Enhancements ## TODO 

Add email notifications (order confirmations, etc.)
Implement real-time order tracking with SignalR
Add review/rating system
Implement search functionality with Elasticsearch
Add admin dashboard
Set up logging with Serilog
Add caching with Redis
Implement rate limiting