# Notes

Contains various notes about the project, which will need to eventually be compiled into into ADRs

## Resource Repository

Manages resource states

### Concerns

#### Should we consider using subresources?

Should we consider storing metadata and spec in a different place than status or whichever subresource we may need in the future?

for:

- isolates storage mechanisms for each potential subresource
- can potentially abstract storages such as MINIO
- patches are focused on a specific part of the resource, which is good for both isolation and 

against:

- complexifies code
- adds new potential software dependencies
- requires additional endpoints
- patches are odd like in k8s, because even if we patch a subresource, we will need to only care about the patches regarding the subresources (ex: /status/*)

### Interface

- Add resource definition
- Get resource definition
- Query resource definition
- Update resource definition
- Delete resource definition

- Add resource
- Get resource
- Query resources
- Update resource (metadata, spec)
- Update subresource (status, data, ...)
- Delete resource

- Save changes

## Resource Registry

Manages well known, read-only resources and definitions. Can only be altered by built-in APIs at startup time (should it though?).

### Interface

- Get resource definition
- Get resource