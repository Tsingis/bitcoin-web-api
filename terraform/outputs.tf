output "container_app_url" {
  value       = azurerm_container_app.ca_app.ingress[0].fqdn
  description = "The ingress URL of the container app"
}
