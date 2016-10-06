#!/bin/bash

# Set variables
IMAGE_NAME=netcore-northwind
IMAGE_VERSION=latest
AWS_DEFAULT_REGION=ap-southeast-2
AWS_ECS_CLUSTER_NAME=default
#AWS_ACCOUNT_NUMBER={} set in private variable
AWS_ECS_REPO_DOMAIN=$AWS_ACCOUNT_NUMBER.dkr.ecr.$AWS_DEFAULT_REGION.amazonaws.com

ECS_SERVICE=$IMAGE_NAME-service
ECS_TASK=$IMAGE_NAME-task
