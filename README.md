# milleniumservice_rec_task
Recruitment task for Kirill Karpenka for Millennium service Dotnet developer


# Fibonacci Calculation System

## Overview

This project implements a distributed system for calculating Fibonacci numbers with the following components:

1. **REST Service**  
   - Calculates Fibonacci numbers via REST API.  
   - Returns an error every 10th request.

2. **SOAP Service**  
   - Calculates Fibonacci numbers via SOAP API.  
   - Simulates long processing time asynchronously.

3. **Integration Service**  
   - Calls REST and SOAP services.  
   - Caches responses in Redis for 5 minutes to reduce load.

4. **Frontend (React)**  
   - Simple UI to input Fibonacci index and display results from the integration service.

## Technologies

- Backend: .NET 8 (ASP.NET Core)  
- SOAP: SoapCore (server), manual HttpClient calls  
- Cache: Redis (via StackExchange.Redis)  
- Frontend: React.js  
- Redis run on free redis instance.
