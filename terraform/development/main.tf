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

