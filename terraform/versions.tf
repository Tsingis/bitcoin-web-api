terraform {
  required_version = ">= 1.13.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 4.42.0"
    }

    external = {
      source  = "hashicorp/external"
      version = ">= 2.3.5"
    }
  }
}
