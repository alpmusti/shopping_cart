# Shopping Cart [![Build Status](https://travis-ci.com/alpmusti/shopping_cart.svg?branch=master)](https://travis-ci.com/alpmusti/shopping_cart)

This project implements second case of Trendyol Codility assignment.

## Get Started

We will talk about dependencies, installation and project structure with detailed in the below.

### Dependencies

In order to build and run this project you must install dependencies which are listed below.

- .NET Core 3.1
- xUnit
- Moq

### Installation

You can flow below steps to build and run this project.

1. Clone this project

`git clone https://github.com/alpmusti/shopping_cart.git`

2. Restore missing NuGet packages before building.

`dotnet restore`

3. To build this project.

`dotnet build`

4. To Run this project

`dotnet run`

**Note:** To run tests of this project you need to switch to test directo and run command below.

`dotnet test`

### Project Design

- Categories may or may not has parent category.
- Products belong to category.
- Campaigns can be applied to the category with some conditions.
- Coupon can be applied to the cart with some conditions.(We apply campaigns first then coupon)
- We calculate delivery cost dynamically using `IDeliveryCostCalculator` interface based on deliveries and number of products.
