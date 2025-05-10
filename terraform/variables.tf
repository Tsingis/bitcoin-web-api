variable "subscription_id" {
  type      = string
  sensitive = true
}

variable "tenant_id" {
  type      = string
  sensitive = true
}

variable "rg_name" {
  type = string
}

variable "kv_name" {
  type = string
}

variable "law_name" {
  type = string
}

variable "ca_env_name" {
  type = string
}

variable "ca_app_name" {
  type = string
}

variable "registry_server" {
  type    = string
  default = "docker.io"
}

variable "registry_username" {
  type      = string
  sensitive = true
}

variable "registry_password" {
  type      = string
  sensitive = true
}

variable "registry_image" {
  type = string
}

variable "container_port" {
  type    = number
  default = 8080
}

variable "environment" {
  type    = string
  default = "Production"
}

