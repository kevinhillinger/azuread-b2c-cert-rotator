#!/bin/bash

config=$(cat scripts/deploy.json | jq .)

suffix=8
resource_group=azureadb2c-management
location=westus2

az group create -n $resource_group -l $location

echo "Deploying resources to:"
echo "  Resource Group: $resource_group"
echo "  Location: $location"

# function app
echo ""
echo "Creating azure function"
func_storage_name="b2cutilstore$suffix"
func_app=b2cutil-functionapp-$suffix

az storage account create \
    --name $func_storage_name \
    --location $location \
    -g $resource_group \
    --sku Standard_LRS \
    --kind StorageV2

az functionapp create -g $resource_group  \
    --consumption-plan-location $location \
    -n $func_app \
    -s $func_storage_name \
    --runtime dotnet \
    --functions-version 3 \
    --runtime-version 12 \
    --os-type Linux

# app settings so it can run

az functionapp config appsettings set \
    --name $func_app \
    --resource-group $resource_group \
    --settings "B2C_CLIENT_ID=$client_id B2C_CLIENT_SECRET=$client_secret  B2C_TENANT_ID=$tenant_id"


echo "Creating logic app"
logic_app=bfyoc-logicapp-notify-$suffix

az logic workflow create \
    --definition ./azure/logicapp.notify.definition.json \
    --location $location \
    --name $logic_app \
    --resource-group $resource_group


echo "Deploying the function app to Azure."
cd src/Functions
func azure functionapp publish $func_app