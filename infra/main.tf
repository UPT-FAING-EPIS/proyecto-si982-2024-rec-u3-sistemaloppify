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
    type = string
    description = "Azure subscription id"
}

variable "sqladmin_username" {
    type = string
    description = "Administrator username for server"
}

variable "sqladmin_password" {
    type = string
    description = "Administrator password for server"
}

variable "db_name" {
  type = string
  description = "Database name"
}

variable "db_username" {
  type = string
  description = "Database username"
}

variable "db_password" {
  type = string
  description = "Database password"
}

provider "azurerm" {
  features {}
  subscription_id = var.suscription_id
}

provider "github" {
  token = var.github_token
}

# Generate a random integer to create a globally unique name
resource "random_integer" "ri" {
  min = 100
  max = 999
}

# Create the resource group
resource "azurerm_resource_group" "rg" {
  name     = "upt-arg-${random_integer.ri.result}"
  location = "centralus"
}

# Create the Linux App Service Plan
resource "azurerm_service_plan" "appserviceplan" {
  name                = "upt-asp-${random_integer.ri.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"
}

# Create the web app, pass in the App Service Plan ID
resource "azurerm_linux_web_app" "webapp" {
  name                  = "upt-proj-${random_integer.ri.result}"
  location              = azurerm_resource_group.rg.location
  resource_group_name   = azurerm_resource_group.rg.name
  service_plan_id       = azurerm_service_plan.appserviceplan.id
  depends_on            = [azurerm_service_plan.appserviceplan]
  //https_only            = true
  site_config {
    minimum_tls_version = "1.2"
    always_on = false
    application_stack {
      docker_image_name = "kyans8/loop:latest"
      docker_registry_url = "https://index.docker.io"      
    }
  }
}

resource "azurerm_mssql_server" "sqlsrv" {
  name                         = "upt-dbs-${random_integer.ri.result}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sqladmin_username
  administrator_login_password = var.sqladmin_password
}

resource "azurerm_mssql_firewall_rule" "sqlaccessrule" {
  name             = "PublicAccess"
  server_id        = azurerm_mssql_server.sqlsrv.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "255.255.255.255"
}

resource "azurerm_mssql_database" "sqldb" {
  name      = "loop"
  server_id = azurerm_mssql_server.sqlsrv.id
  sku_name = "Free"
}

# Crear los secretos de GitHub
resource "github_actions_secret" "db_name" {
  repository    = "tomasyoel/automatizacionloopify"
  secret_name   = "DB_NAME"
  plaintext_value = bdloop
}

resource "github_actions_secret" "db_username" {
  repository    = "tomasyoel/automatizacionloopify"
  secret_name   = "DB_USERNAME"
  plaintext_value = bdloop
}

resource "github_actions_secret" "db_password" {
  repository    = "tomasyoel/automatizacionloopify"
  secret_name   = "DB_PASSWORD"
  plaintext_value = loopifyplus
}