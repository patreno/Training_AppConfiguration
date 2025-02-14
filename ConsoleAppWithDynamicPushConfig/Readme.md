## Dynamic Configuration with Push model

Example based on https://learn.microsoft.com/en-us/azure/azure-app-configuration/enable-dynamic-configuration-dotnet-core-push-refresh

### Usage

#### Set App Configuration variables

Set environment variable `Training_AppConfig_Endpoint` with the endpoint value of your Azure App Config

e.g. `$env:Training_AppConfig_Endpoint='https://<Your name>.azconfig.io`

#### Set Service Bus Topic variables

Set followint variable names:

- Training_ServiceBusNamespace
    e.g. `$env:Training_ServiceBusNamespace='<your name>.servicebus.windows.net'`
- Training_ServiceBusTopic
    e.g. `$env:Training_ServiceBusTopic='myTopic'`
- Training_ServiceBusSubscription
    e.g. `$env:Training_ServiceBusSubscription='mySubscription'`


### Messages

The event Grid emits message to the SB Topic in this format:

The following example shows the schema of a key-value modified event:

```json
[{
  "id": "84e17ea4-66db-4b54-8050-df8f7763f87b",
  "source": "/subscriptions/aaaa0a0a-bb1b-cc2c-dd3d-eeeeee4e4e4e/resourceGroups/testrg/providers/microsoft.appconfiguration/configurationstores/contoso",
  "subject": "https://contoso.azconfig.io/kv/Foo?label=FizzBuzz",
  "data": {
    "key": "Foo",
    "label": "FizzBuzz",
    "etag": "FnUExLaj2moIi4tJX9AXn9sakm0"
  },
  "type": "Microsoft.AppConfiguration.KeyValueModified",
  "time": "2019-05-31T20:05:03Z",
  "specversion": "1.0"
}]
```


See for more info: https://learn.microsoft.com/en-us/azure/event-grid/event-schema-app-configuration?tabs=cloud-event-schema