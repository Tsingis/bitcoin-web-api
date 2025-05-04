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

variable "law_name" {
  type = string
}

variable "ca_env_name" {
  type = string
}

variable "ca_app_name" {
  type = string
}

variable "container_port" {
  type    = number
  default = 8080
}
