# Hylo

Hylo is a free, vendor-agnostic, open-source solution for resource-oriented architecture management. It is designed to help users manage resources in a database by providing a hybrid solution that combines different database technologies and approaches.

One of the main goals of Hylo is to provide a high-level abstraction for Kubernetes resource management. While Kubernetes provides powerful primitives for deploying and managing containerized applications, it can be challenging to work with for simple use cases or in lean or legacy environments.

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

You can now start using the `Hylo.Infrastructure.IRepository` service to:

*Create a new resource:*

```c#
var metadata = new ResourceMetadata()
{
  Name = "belgian-shepherd",
  Namespace = "dogs",
  Labels = new Dictionary<string, string>[{ "petstore.swagger.io/category": "shepherd" }]
};
var spec = new()
{
  category = new PetCategory("shepherd"),
  photoUrls = new List<Uri>(){ new Uri("https://t3.gstatic.com/licensed-image?q=tbn:ANd9GcQF1CVgqJEqPH68s2Ml0y2ERG_Amu2eubjWg-Vpm0Ok5wXP5mu6cxh8BmPwsoDahreS26s-2pOwhdvmf1w") }
};
var status = new()
{
  status = "available"
};
var pet = new Pet(metadata, spec, status);
await repository.CreateResourceAsync(pet);
```

*Get a resource:*

```c#
var pet = await repository.GetResourceAsync<Pet>("belgian-shepherd", "dogs");
```

*Enumerate resources:*

```c#
await foreach(var pet in await repository.GetResourcesAsync<Pet>("dogs"))
{
  ...
}
```

*List resources:*

```c#
var list = await repository.ListResourcesAsync<Pet>("dogs");
```

*Watch resources:*

```c#
using var watch = await repository.WatchResourcesAsync<Pet>("dogs");
watch.Subscribe(e => 
{
  Console.WriteLine($"Event of type {e.Type} received for resource {e.Resource}");
});
```

*Replace resource:*

```c#
await repository.ReplaceResourceAsync(updatedPet);
```

*Replace subresource:*

```c#
await repository.ReplaceSubResourceAsync(updatedPet, "status");
```

*Patch resource:*

```c#
var jsonPatch = ...;
var patch = new Patch(PatchType.JsonPatch, jsonPatch);
await repository.PatchResourceAsync<Pet>(patch, "belgian-shepherd", "dogs");
```

*Patch subresource:*

```c#
var jsonPatch = ...;
var patch = new Patch(PatchType.JsonPatch, jsonPatch);
await repository.PatchSubResourceAsync<Pet>(patch, "belgian-shepherd", "status", "dogs");
```

*Delete resource:*

```c#
await respository.DeleteResourceAsync<Pet>("belgian-shepherd", "dogs");
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
                category:
                  type: object
                  properties:
                    id:
                      type: integer
                    name:
                      type: string
                  required:
                    - name
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

Hylo is licensed under [Apache 2.0](LICENSE).
