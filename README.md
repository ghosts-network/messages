# GhostNetwork - Messages

Messages is a part of GhostNetwork education project

## Installation

copy provided docker-compose.yml and customize for your needs

compile images from the sources - `docker-compose build && docker-compose up -d`

### Parameters

| Environment                    | Description                                               |
|--------------------------------|-----------------------------------------------------------|
| MONGO_CONNECTION               | Connection string to MongoDb instance                     |
| PROFILES_ADDRESS               | Adress to profile microservice                            |

## Development

To run dependent environment use

```bash
docker-compose -f dev-compose.yml pull
docker-compose -f dev-compose.yml up --force-recreate
```
