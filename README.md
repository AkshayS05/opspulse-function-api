# OpsPulse — Serverless Case API

A serverless **Azure Function (C#, .NET isolated)** that exposes case-management 
data from an **Azure SQL** database as a secured JSON API. Part of OpsPulse, a 
cloud data-pipeline project.

## What it does

An HTTP request hits the function → it connects to Azure SQL → queries case 
records → returns them as structured JSON.

## Architecture
