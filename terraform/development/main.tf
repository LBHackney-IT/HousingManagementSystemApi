provider "aws" {
    region  = "eu-west-2"
    version = "~> 2.0"
}
data "aws_caller_identity" "current" {}
data "aws_region" "current" {}


terraform {
    backend "s3" {
        bucket  = "terraform-state-housing-development"
        encrypt = true
        region  = "eu-west-2"
        key     = "services/housing-repairs-online/state"
    }
}


resource "aws_ssm_parameter" "authentication_identifier" {
    name  = "HousingManagementSystemApi/authentication-identifier/development"
    type  = "String"
    value = var.authentication_identifier
}
resource "aws_ssm_parameter" "jwt_secret" {
    name  = "HousingManagementSystemApi/jwt-secret/development"
    type  = "String"
    value = var.jwt_secret
}
resource "aws_ssm_parameter" "universal_housing_connection_string" {
    name  = "HousingManagementSystemApi/universal-housing-connection-string/development"
    type  = "String"
    value = var.universal_housing_connection_string
}

