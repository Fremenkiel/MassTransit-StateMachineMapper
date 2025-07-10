# MassTransit-StateMachineMapper
Mappes a MassTransit State Machine based on a json convertible entity.

## Reasoning for the existence of this project
Have you ever read the documentation of MassTransit and thought, this is an absolutely amazing library, but what if you could define and create a state machine based on a JSON formatted workflow?

This would allow you to have a graphical builder whose input could be translated into a state machine.

Well... I, too, had that thought and weren't able to find anything examples that could get me closer to my goal. As a result of that, this project was born. 

This project allows you to take a JSON convertable and database compatible entity, and convert it to a functional MassTransit State Machine, allowing you to change the structure of the flow semi-dynamicly.

This allows you to convert a graphical user input to a resulting workflow, without needing to change the code of the program.

## Docker
The project contains a docker build file for a postgres instance used in this example.

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
As the program starts up, the state machine is added to the bus, ready for a template input to be passed to it.

When the POST /api/state-machine/ endpoint is called with a Guid in the body, the DynamicStateMachineManager checks the database for any template with the same Id.

If any is found, then a new endpoint is created in MassTransit and the template is passed into the OnboardingStateMachine which builds the state machine based on the template entries.

Now the state machine is available as a normal state machine through the Guid endpoint.

When it's time to remove the state machine again, then the DELETE /api/state-machine/{queryName} comes in handy?

This endpoint stops the HostReceiveEndpointHandle build by the DynamicStateMachineManager, and sends a DELETE request to RabbitMq to remove the queue and exchange, identified by the Guid, from there as well.

The other exchanges created for the commands end events are not removed, as these will be reused by any other endpoint created.

## Credits
Massive credits to Chris Patterson for creating and maintaining the MassTransit library, the existence of it made my life a whole lot easier.

Credit to milanjovanovic.tech for the base example which this project is based on.
