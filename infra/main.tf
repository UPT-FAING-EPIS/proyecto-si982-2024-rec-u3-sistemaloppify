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

provider "azurerm" {
  features        {}
  subscription_id = var.suscription_id
}

# Generar un número aleatorio para crear nombres únicos
resource "random_integer" "ri" {
  min = 100
  max = 999
}

# Crear el Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "upt-arg-${random_integer.ri.result}"
  location = "centralus"
}

# Crear el App Service Plan para Linux
resource "azurerm_service_plan" "appserviceplan" {
  name                = "upt-asp-${random_integer.ri.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"
}

# Crear la Web App
resource "azurerm_linux_web_app" "webapp" {
  name                  = "upt-awa-${random_integer.ri.result}"
  location              = azurerm_resource_group.rg.location
  resource_group_name   = azurerm_resource_group.rg.name
  service_plan_id       = azurerm_service_plan.appserviceplan.id
  depends_on            = [azurerm_service_plan.appserviceplan]
  site_config {
    minimum_tls_version = "1.2"
    always_on           = false
    application_stack {
      docker_image_name   = "kyans8/shorten:latest"
      docker_registry_url = "https://index.docker.io"
    }
  }
}

# Obtener los secretos de GitHub
data "github_actions_secret" "db_name" {
  repository = "tomasyoel/automatizacionloopify"
  secret_name = "DB_NAME"
}

data "github_actions_secret" "db_username" {
  repository = "tomasyoel/automatizacionloopify"
  secret_name = "DB_USERNAME"
}

data "github_actions_secret" "db_password" {
  repository = "tomasyoel/automatizacionloopify"
  secret_name = "DB_PASSWORD"
}

# Crear el Servidor SQL en Azure
resource "azurerm_mssql_server" "sqlsrv" {
  name                         = "upt-dbs-${random_integer.ri.result}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = data.github_actions_secret.db_username.secret_value
  administrator_login_password = data.github_actions_secret.db_password.secret_value
}

# Crear la regla de firewall para acceso público
resource "azurerm_mssql_firewall_rule" "sqlaccessrule" {
  name             = "PublicAccess"
  server_id        = azurerm_mssql_server.sqlsrv.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "255.255.255.255"
}

# Crear la base de datos en el servidor SQL
resource "azurerm_mssql_database" "sqldb" {
  name      = data.github_actions_secret.db_name.secret_value
  server_id = azurerm_mssql_server.sqlsrv.id
  sku_name  = "Free"
}
