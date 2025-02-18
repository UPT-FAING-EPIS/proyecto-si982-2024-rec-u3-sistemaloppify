terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0.0"
    }
  }
  required_version = ">= 0.14.9"
}

variable "suscription_id" {
  type        = string
  description = "Azure subscription id"
}

variable "sqladmin_username" {
  type        = string
  description = "Administrator username for server"
}

variable "sqladmin_password" {
  type        = string
  description = "Administrator password for server"
}

provider "azurerm" {
  features {}
  subscription_id = var.suscription_id
}

# Random integer for unique naming
resource "random_integer" "ri" {
  min = 100
  max = 999
}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "loopify_group"
  location = "East US"
}

# App Service Plan (Windows)
resource "azurerm_service_plan" "appserviceplan" {
  name                = "ASP-loopifygroup-${random_integer.ri.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Windows"
  sku_name            = "F1"
}

# Web App for ASP.NET V4.8
resource "azurerm_windows_web_app" "webapp" {
  name                = "loopify"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.appserviceplan.id

  site_config {
  ftps_state = "Disabled"
  always_on  = true
}


  identity {
    type = "SystemAssigned"
  }

  https_only = true

}

# SQL Server
resource "azurerm_mssql_server" "sqlsrv" {
  name                         = "loopify-sql-${random_integer.ri.result}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sqladmin_username
  administrator_login_password = var.sqladmin_password
  public_network_access_enabled = true
}

# SQL Database
resource "azurerm_mssql_database" "sqldb" {
  name      = "loopifydb"
  server_id = azurerm_mssql_server.sqlsrv.id
  sku_name  = "Basic"
}

# Public Access to SQL Server
resource "azurerm_mssql_firewall_rule" "sqlaccessrule" {
  name             = "PublicAccess"
  server_id        = azurerm_mssql_server.sqlsrv.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "255.255.255.255"
}
