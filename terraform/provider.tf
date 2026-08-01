terraform {
  backend "azurerm" {
    use_cli          = true
    use_azuread_auth = true
    key              = "terraform.tfstate"
  }
}

provider "azurerm" {
  subscription_id                 = var.subscription_id
  tenant_id                       = var.tenant_id
  resource_provider_registrations = "legacy"
  features {
    enhanced_validation {
      locations          = true # Re-enable location validation at plan time
      resource_providers = true # Re-enable resource provider validation at plan time
    }
  }
}
