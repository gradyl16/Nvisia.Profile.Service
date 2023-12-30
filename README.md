# nvisia (n)Ternship

### Winter 2023

## Nvisia Profile Service API

The Profile Service is part of a larger project that will store employee profiles. These profiles can be output as one-pagers that can then be distributed.

Nvisia is a software consulting company (SaaS) based out of Chicago. The project is hosted by Azure Cloud Computing Services and managed by a Kubernetes cluster. The motivation behind this project is to facilitate the ease of marketing nvisia's consultants to potential clients by reducing the manual and often tedious modifications that need to be done on a regular basis to appeal to any particular client. Note that this project is still in development and unavailable to the public.

## Getting Started
1.	Installation process
      Required:
- .NET Core 8
- Docker
- Nuget

2.	Software dependencies
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [AutoMapper](https://docs.automapper.org/en/latest/index.html)
- [FluentValidation](https://fluentvalidation.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [NSubstitute](https://nsubstitute.github.io/)
- [nUnit](https://nunit.org/)

3.	Latest releases

4.	API references
- The application uses swagger to expose the APIs locally.

## Build and Test
<b>Make sure you have run dotnet dev-certs https --trust<b>

Running:
```
cd src
dotnet run 
```

Access:
```
https://localhost:5001/swagger
```

# Skaffold & Kubernetes

You can run the application on your local development machine in Kubernetes! Use Skaffold to easily manage your K8s resources. Running the application in Kubernetes on your local machine allows you to test the application's build, deploy, and running states against its target environment in the cloud.

First, navigate to `_deploy/kustomize/overlays/local` and open `pvc.yaml`. Change the `path` to a desired location on your local machine for persistent database storage.

Next, run `skaffold dev -p local` to deploy the application to your local docker-desktop k8s cluster.

Access:
```
http://localhost/swagger
```
