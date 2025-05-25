terraform {
  backend "azurerm" {
    use_cli          = true
    use_azuread_auth = true
    key              = "terraform.tfstate"
  }
}

provider "azurerm" {
  subscription_id = var.subscription_id
  tenant_id       = var.tenant_id
  features {}
}
