# GreenFlux Smart Charging API
**Assignment Version 14**

This API exposes a simplified smart charging domain.

Domain Model:
[Domain model](documentation/domainmodel.png)
- **Group**: has a unique Identifier (cannot be changed), Name (can be changed), Capacity in Amps
(integer, value greater than zero, can be changed). A Group can contain multiple charge stations.
- **Charge station**: has a unique Identifier (cannot be changed), Name (can be changed), and Connectors
(at least one, but not more than 5).
- **Connector**: has integer Identifier unique within the context of a charge station with (possible range
of values from 1 to 5), Max current in Amps (integer, value greater than zero, can be changed).

Add Groups with the _/groups/_ route.

Add Charge Stations with the _/groups/{groupId}/stations/_ route.

Add Connectors with the _/groups/{groupId}/stations/{stationId}/connectors_.

Alternatively, you can also add or update connectors directly when adding or updating Charge Stations.

## How To Run Locally
### Pre-requisites
1. dotnetcore 7.0 or higher

### Step by step
1. Execute the _run.sh_ shell script located at the root folder of the repo. Alternatively you can run `dotnet run --project src/SmartCharging.Api --launch-profile http` in your terminal or console.
1. The API will be exposed at localhost:5094. Go to http://localhost:5094/swagger/index.html to open Swagger UI.
