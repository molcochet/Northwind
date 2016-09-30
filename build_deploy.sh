#!/bin/bash

# install dependencies
sudo apt-get install jq #install jq for json parsing
pip install --user awscli # install aws cli w/o sudo
export PATH=$PATH:$HOME/.local/bin # put aws in the path

# Set variables
IMAGE_NAME=netcore-northwind
IMAGE_VERSION=latest
ECS_SERVICE=netcoreapps-northwind-service
ECS_TASK=netcoreapps-northwind-task
AWS_DEFAULT_REGION=ap-southeast-2
AWS_ECS_CLUSTER_NAME=default
#AWS_ECS_REPO_DOMAIN={} set in private variable

# Build/Deploy process
eval $(aws ecr get-login) #needs AWS_ACCESS_KEY_ID and AWS_SECRET_ACCESS_KEY envvars
docker build -t $IMAGE_NAME .
docker tag $IMAGE_NAME $AWS_ECS_REPO_DOMAIN/$IMAGE_NAME:$IMAGE_VERSION
docker push $AWS_ECS_REPO_DOMAIN/$IMAGE_NAME:$IMAGE_VERSION
aws ecs register-task-definition --cli-input-json file://task-definition.json > /dev/null # Create a new task revision
TASK_REVISION=$(aws ecs describe-task-definition --task-definition $ECS_TASK | jq '.taskDefinition.revision') #get latest revision
aws ecs update-service --cluster $AWS_ECS_CLUSTER_NAME --service $ECS_SERVICE --task-definition "$ECS_TASK:$TASK_REVISION" > /dev/null #update service with latest task revision
TEMP_ARN=$(aws ecs list-tasks --service-name $ECS_SERVICE | jq '.taskArns[0]') # Get current running task ARN
TASK_ARN="${TEMP_ARN%\"}" # strip double quotes
TASK_ARN="${TASK_ARN#\"}" # strip double quotes
aws ecs stop-task --task $TASK_ARN > /dev/null # Stop current task to force start of new task revision with new image