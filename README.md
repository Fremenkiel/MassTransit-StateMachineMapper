# MassTransit-StateMachineMapper
Mappes a MassTransit State Machine based on a json convertible entity.

## Reasoning for the existence of this project
This project allows you to take a json convertable and database compatible entity, and convert it to a functional MassTransit State Machine, allowing you to change the structure of the flow semi-dynamicly.

This allows you to convert a graphical user input to a resulting workflow, without needing to change the code of the program.

## Docker
The project contains a docker build file for a postgrsql instance used in this example.

1. Start by building the docker file, to generate the image used in the docker compose file.

```
cd ./docker/

docker buildx build --platform linux/amd64 --rm -f ./Dockerfile -t state-machine-mapper/masstransit/backend/preloaded-db:latest .
```

2. Run the composer file to start the postgresql and rabbitMq containers used in this project.

```
docker-compose -f ./docker/compose.yml up -d
```

## What happens?
When the program is run, the state machine is build based on the SagStateMachineTemplate located in the BehaviorDefiner.

To stop the mapping at startup, and postpone it for later mapping, remove the following line from the program.cs file:
```
cfg.ConfigureEndpoints(context);
```

This stoppes MassTransit from adding the endpoints af startup, and allows you to do this manually when needed.

## Credits
Credit to milanjovanovic.tech for the base example which this project is based on.
