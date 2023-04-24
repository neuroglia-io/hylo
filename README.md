# Hylo

Hylo is a free, vendor-agnostic, open-source solution for resource-oriented architecture management. It is designed to help users manage resources in a database by providing a hybrid solution that combines different database technologies and approaches.

## Features

- Support for multiple database technologies
- Flexible resource modeling and management
- Integration with existing database infrastructure
- Customizable resource queries and endpoints
- Comprehensive logging and analytics

## Database Providers

Hylo currently supports the following database providers:

- File System
- Kubernetes
- Mongo
- Redis

## Installation

### Embedded

To use Hylo in your application, you must first add the `Hylo.Infrastructure` nuget package:

```bash
dotnet add package Hylo.Infrastructure
```


Then, you must set the [database provider](#database-providers) you want to use by using one of the following strategies:
- Configure the `IDatabaseProvider` to use by calling the `IRepositoryOptionsBuilder.UseDatabaseProvider<TProvider>` method
- Downloading a [database provider](#database-providers) implementation in the `plugins` folder of your application's output directory

Finally, you must add and configure Hylo services:

```c#
  services.AddHylo(configuration, builder => 
  {
    builder.UsePluginBasedProvider();
  });
```

## Usage

To start using Hylo, you first need to define your resources in a resource model file. This file defines the structure of your resources, including their properties, relationships, and other metadata.

Once you have defined your resource model, you can start Hylo and use its API to manage your resources. Hylo provides a label-based query language that allows you to query and manipulate your resources using a RESTful API.

*Example definition of a pet resource:*

```yaml
apiVersion: hylo.io/v1
kind: ResourceDefinition
metadata:
  name: pets.petstore.swagger.io # must be equal to '{plural}.{group}'
spec:
  scope: Namespaced #possible values are: Namespaced, Cluster
  group: petstore.swagger.io #the API group the defined resources belong to. Must be a valid subdomain namespace
  names:
    singular: pet #the singular form of the defined resource type name
    plural: pets #the plural form of the defined resource type name
    kind: Pet #the defined resource type name
  versions:
    - name: v1 #the name of the version. Must be alphanumeric
      served: true #indicates whether the API serves the resource's version
      storage: true #indicates whether the version is the storage version. Exactly one version must have the property set to 'true'
      schema:
        openAPIV3Schema: #the OpenAPI V3 schema used to validate defined resources
          type: object
          properties:
            spec:
              type: object
              properties:
                id:
                  type: integer
                category:
                  type: object
                  properties:
                    id:
                      type: integer
                    name:
                      type: string
                  required:
                    - name
                name:
                  type: string
                photoUrls:
                  type: array
                  items:
                    type: string
                tags:
                  type: array
                  items:
                    type: object
                    properties:
                      id:
                        type: integer
                      name:
                        type: string
                    required:
                      - name
              required:
                - id
                - category
                - name
            status:
              type: object
              properties:
                status:
                  type: string
                  enum: [ available, pending, sold]
      subresources: #defines the subresources of defined resources (any top level properties other than metadata and spec. i.e. 'status')
        status: {}
```

## Contributing

Hylo is an open-source project, and contributions are always welcome! If you want to contribute to Hylo, you can start by checking out the [contributing guidelines](CONTRIBUTING.md) and the [code of conduct](CODE_OF_CONDUCT.md).

## License

Hylo is licensed under [Apache 2.0](LICENSE.MD).
