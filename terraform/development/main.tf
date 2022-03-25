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
        key     = "services/housing-repairs-online/state" #e.g. "services/transactions-api/state"
    }
}


resource "aws_secretsmanager_secret" "authentication_identifier" {
    name = "HousingManagementSystemApi/authentication-identifier/development"
}

resource "aws_secretsmanager_secret" "jwt_secret" {
    name = "HousingManagementSystemApi/jwt-secret/development"
}

resource "aws_secretsmanager_secret" "universal_housing_connection_string" {
    name = "HousingManagementSystemApi/universal-housing-connection-string/development"
}
